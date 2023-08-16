using ClosedXML.Excel;
using DataTransferObjects.VesselsRawData;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using ServiceContracts;
using System.Collections;
using System.Net;
using System.Text;
using System.Xml;
using Vessels.ApiModels;
using Vessels.Helper;

namespace Vessels.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VesselsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceManager _serviceManager;

        private static DateTime _serviceStartedTime;
        private static DateTime _serviceEndedTime;

        private const string vesselsXMLName = "Vessels XML";
        private const string vesselsExcelName = "Vessels Excel";

        public VesselsController(IConfiguration configuration,
            IServiceManager serviceManager)
        {
            _configuration = configuration;
            _serviceManager = serviceManager;
            _serviceStartedTime = DateTime.Now;
            _serviceEndedTime = _serviceStartedTime.AddMinutes(10);
        }


        [HttpGet]
        //It is used to fetch xml reports from email and save it in database
        public async Task<IActionResult> Index()
        {
            Log.Information($"XML Reading Service Started at: {DateTime.Now}");

            //    Using Microsoft.Identity.Client 4.22.0
            var cca = ConfidentialClientApplicationBuilder
                        .Create(_configuration["MailConfiguration:AppId"])
                        .WithClientSecret(_configuration["MailConfiguration:ClientSecret"])
                        .WithTenantId(_configuration["MailConfiguration:TenantId"])
                        .Build();

            var ewsScopes = new string[] { "https://outlook.office365.com/.default" };

            AuthenticationResult authResult;
            // Configure the ExchangeService with the access token
            var ewsClient = new ExchangeService();

            try
            {
                authResult = await cca.AcquireTokenForClient(ewsScopes).ExecuteAsync();
                ewsClient.Credentials = new OAuthCredentials(authResult.AccessToken);
            }
            catch (Exception ex)
            {
                await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                {
                    ServiceName = vesselsXMLName,
                    StartedDateTime = _serviceStartedTime,
                    EndedDateTime = _serviceEndedTime,
                    ResourceUri = new Uri(ewsScopes[0]),
                    RequestJSONBody = JsonConvert.SerializeObject(ewsScopes[0]),
                    ResponseJSONBody = JsonConvert.SerializeObject(((MsalServiceException)ex).ResponseBody),
                    RecordsToBeProcessed = 0,
                    RecordsActuallyProcessed = 0,
                    Content = null,
                    StatusCode = ((MsalServiceException)ex).StatusCode.ToString(),
                    StatusDescription = ex.Message,
                    Source = ex.Source,
                    Message = ex.Message,
                    InnerException = ex.InnerException?.ToString(),
                    StackTrace = ex?.StackTrace
                });

                Thread.Sleep(30000);

                try
                {
                    authResult = await cca.AcquireTokenForClient(ewsScopes).ExecuteAsync();
                    ewsClient.Credentials = new OAuthCredentials(authResult.AccessToken);
                }
                catch (Exception exAfter30Seconds)
                {
                    await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                    {
                        ServiceName = vesselsXMLName,
                        StartedDateTime = _serviceStartedTime,
                        EndedDateTime = _serviceEndedTime,
                        ResourceUri = new Uri(ewsScopes[0]),
                        RequestJSONBody = JsonConvert.SerializeObject(ewsScopes[0]),
                        ResponseJSONBody = JsonConvert.SerializeObject(((MsalServiceException)exAfter30Seconds).ResponseBody),
                        RecordsToBeProcessed = 0,
                        RecordsActuallyProcessed = 0,
                        Content = null,
                        StatusCode = ((MsalServiceException)exAfter30Seconds).StatusCode.ToString(),
                        StatusDescription = exAfter30Seconds?.Message,
                        Source = exAfter30Seconds?.Source,
                        Message = exAfter30Seconds?.Message,
                        InnerException = exAfter30Seconds?.InnerException?.ToString(),
                        StackTrace = exAfter30Seconds?.StackTrace
                    });

                    return BadRequest(new CommonApiResponse()
                    {
                        data = new(),
                        message = exAfter30Seconds.Message,
                        statusCode = (int)HttpStatusCode.BadRequest
                    });
                }
            }

            // Configure the ExchangeService with the access token
            ewsClient.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            ewsClient.ImpersonatedUserId =
                new ImpersonatedUserId(ConnectingIdType.SmtpAddress, "veslinkapi@ship-watch.com");

            //Include x-anchormailbox header
            ewsClient.HttpHeaders.Add("X-AnchorMailbox", "veslinkapi@ship-watch.com");

            // Make an EWS call
            try
            {
                var folders = ewsClient.FindFolders(WellKnownFolderName.MsgFolderRoot, new FolderView(10));
            }
            catch (Exception ex)
            {
                await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                {
                    ServiceName = vesselsXMLName,
                    StartedDateTime = _serviceStartedTime,
                    EndedDateTime = _serviceEndedTime,
                    ResourceUri = ewsClient.Url,
                    RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                    ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)ex).Response),
                    RecordsToBeProcessed = 0,
                    RecordsActuallyProcessed = 0,
                    Content = null,
                    StatusCode = ((int)((ServiceResponseException)ex).ErrorCode).ToString(),
                    StatusDescription = ex.Message,
                    Source = ex?.Source,
                    Message = ex?.Message,
                    InnerException = ex?.InnerException?.ToString(),
                    StackTrace = ex?.StackTrace
                });

                return BadRequest(new CommonApiResponse()
                {
                    data = new(),
                    message = ex.Message,
                    statusCode = (int)HttpStatusCode.BadRequest
                });
            }

            Folder inbox = Folder.Bind(ewsClient, WellKnownFolderName.Inbox);

            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));

            ItemView view = new(20);

            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);

            bool isMoreItemAvailable = false;
            //fetch exixting attachments ids
            List<string?> DbAttachmentIds = await _serviceManager.VesselRawDataService.GetAllExistedAttachmentIdAsync();

            IEnumerable<string?> imoNumberList = await _serviceManager.VesselMetaDataService.GetActiveImoNumbersAsync();

            var findResults = ewsClient.FindItems(WellKnownFolderName.Inbox, sf, view);
            isMoreItemAvailable = findResults.MoreAvailable;

            List<EmailMessage?> emailslist = new(findResults.TotalCount);
            if (findResults.TotalCount > 0 && findResults.Items != null && findResults.Items.Count > 0)
            {
                var _unreadEmailslist = findResults.Items.Where(x => x != null).Select(x => x as EmailMessage).ToList();
                emailslist = _unreadEmailslist;

                try
                {
                    ewsClient.LoadPropertiesForItems(emailslist, PropertySet.FirstClassProperties);
                }
                catch (Exception ex)
                {
                    await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                    {
                        ServiceName = vesselsXMLName,
                        StartedDateTime = _serviceStartedTime,
                        EndedDateTime = _serviceEndedTime,
                        ResourceUri = ewsClient.Url,
                        RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                        ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)ex).Response),
                        RecordsToBeProcessed = 0,
                        RecordsActuallyProcessed = 0,
                        Content = null,
                        StatusCode = ((int)((ServiceResponseException)ex).ErrorCode).ToString(),
                        StatusDescription = ex.Message,
                        Source = ex?.Source,
                        Message = ex?.Message,
                        InnerException = ex?.InnerException?.ToString(),
                        StackTrace = ex?.StackTrace
                    });

                    try
                    {
                        Thread.Sleep(5000);
                        ewsClient.LoadPropertiesForItems(emailslist, PropertySet.FirstClassProperties);
                    }
                    catch (Exception exAfter5Seconds)
                    {
                        await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                        {
                            ServiceName = vesselsXMLName,
                            StartedDateTime = _serviceStartedTime,
                            EndedDateTime = _serviceEndedTime,
                            ResourceUri = ewsClient.Url,
                            RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                            ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)exAfter5Seconds).Response),
                            RecordsToBeProcessed = 0,
                            RecordsActuallyProcessed = 0,
                            Content = null,
                            StatusCode = ((int)((ServiceResponseException)exAfter5Seconds).ErrorCode).ToString(),
                            StatusDescription = exAfter5Seconds.Message,
                            Source = exAfter5Seconds?.Source,
                            Message = exAfter5Seconds?.Message,
                            InnerException = exAfter5Seconds?.InnerException?.ToString(),
                            StackTrace = exAfter5Seconds?.StackTrace
                        });
                    }

                    return BadRequest(new CommonApiResponse()
                    {
                        data = new(),
                        message = ex?.Message,
                        statusCode = (int)HttpStatusCode.BadRequest
                    });
                }

                IList emailSubjectsForXML = _configuration.GetSection("EmailSubjectsForXML").Get<string[]>();

                var successfullyCompletedEmails = new List<EmailMessage>();

                Log.Information($"Total {findResults.Items.Count} mails to be read.");

                int index = 0;

                foreach (var emailMessageItem in findResults.Items)
                {
                    if (emailSubjectsForXML.Contains(emailMessageItem.Subject))
                    {
                        try
                        {
                            #region Attachments

                            try
                            {
                                try
                                {
                                    EmailMessage.Bind(ewsClient, emailMessageItem.Id, new PropertySet(ItemSchema.Attachments));
                                }
                                catch (Exception ex)
                                {
                                    await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                                    {
                                        ServiceName = vesselsXMLName,
                                        StartedDateTime = _serviceStartedTime,
                                        EndedDateTime = _serviceEndedTime,
                                        ResourceUri = ewsClient.Url,
                                        RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                                        ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)ex).Response),
                                        RecordsToBeProcessed = findResults.Items.Count,
                                        RecordsActuallyProcessed = index,
                                        Content = null,
                                        StatusCode = ((int)((ServiceResponseException)ex).ErrorCode).ToString(),
                                        StatusDescription = ex.Message,
                                        Source = ex?.Source,
                                        Message = ex?.Message,
                                        InnerException = ex?.InnerException?.ToString(),
                                        StackTrace = ex?.StackTrace
                                    });

                                    try
                                    {
                                        Thread.Sleep(5000);
                                        EmailMessage.Bind(ewsClient, emailMessageItem.Id, new PropertySet(ItemSchema.Attachments));
                                    }
                                    catch (Exception exAfter5Seconds)
                                    {
                                        await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                                        {
                                            ServiceName = vesselsXMLName,
                                            StartedDateTime = _serviceStartedTime,
                                            EndedDateTime = _serviceEndedTime,
                                            ResourceUri = ewsClient.Url,
                                            RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                                            ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)exAfter5Seconds).Response),
                                            RecordsToBeProcessed = findResults.Items.Count,
                                            RecordsActuallyProcessed = index,
                                            Content = null,
                                            StatusCode = ((int)((ServiceResponseException)exAfter5Seconds).ErrorCode).ToString(),
                                            StatusDescription = exAfter5Seconds.Message,
                                            Source = exAfter5Seconds?.Source,
                                            Message = exAfter5Seconds?.Message,
                                            InnerException = exAfter5Seconds?.InnerException?.ToString(),
                                            StackTrace = exAfter5Seconds?.StackTrace
                                        });
                                    }
                                }

                                var xmlAttachments = emailMessageItem.Attachments;

                                foreach (Attachment attachment in xmlAttachments)
                                {
                                    if (attachment.ContentType != "text/xml" || (DbAttachmentIds != null && DbAttachmentIds.Count > 0 && DbAttachmentIds.Contains(attachment.Id)))
                                    {
                                        // Mail is being marked as read even though it is invalid, because, next time, if we take the descending orderwise batch, then this invalid mail won't be coming.
                                        successfullyCompletedEmails.Add((EmailMessage)emailMessageItem);

                                        await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                                        {
                                            ServiceName = vesselsXMLName,
                                            Description = attachment.ContentType != "text/xml" ?
                                                                                    "The received email is not having expected file." : "This email attachment report file was already read",
                                            EmailSubject = string.Join(" - ", "Invalid File", emailMessageItem.Subject),
                                            EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                                            EmailTime = emailMessageItem.DateTimeReceived,
                                            FileName = attachment.Name
                                        });

                                        continue;
                                    }

                                    if (attachment is FileAttachment _attachment)
                                    {
                                        //var _attachment = (FileAttachment)attachment;

                                        _attachment.Load();
                                        string xmlDocument = Encoding.UTF8.GetString(_attachment.Content);

                                        XmlDocument xml = new();
                                        xml.LoadXml(xmlDocument);

                                        string Json = JsonConvert.SerializeXmlNode(xml);

                                        if (!string.IsNullOrEmpty(Json) && !string.IsNullOrWhiteSpace(Json))
                                        {
                                            ReportResponse? reportResponse = JsonConvert.DeserializeObject<ReportResponse>(Json);

                                            if (reportResponse != null && reportResponse.Form != null)
                                            {
                                                Form form = reportResponse.Form;
                                                VesselsRawDataDto vesselsRawDataDto = new();

                                                //vem-195
                                                DateTime? form_reportTime = ValidateReportData.GetDateTimeFromOffset(form.ReportTime);

                                                var startingDateVesselsReport = Convert.ToDateTime("2023-07-01T00:00:00");

                                                bool isImoInActiveVessel = false;

                                                //vem-195
                                                bool isImoValid = ValidateReportData.ValidateVesselImoNumber(form.ImoNumber, imoNumberList, out isImoInActiveVessel);

                                                //vem-195
                                                if (!ValidateReportData.ValidateFormIdentifier(form.FormIdentifier) ||
                                                    form_reportTime == null || form_reportTime < startingDateVesselsReport ||
                                                    !isImoValid)
                                                {
                                                    // Mail is being marked as read even though it is invalid, because next time if we take the descending orderwise batch, then this invalid mail won't be coming.
                                                    successfullyCompletedEmails.Add((EmailMessage)emailMessageItem);

                                                    await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                                                    {
                                                        ServiceName = vesselsXMLName,
                                                        Description = isImoInActiveVessel ? "The received email is not having expected Format." : "The received email's imo number is not either in correct format or in active vessels' list.",
                                                        EmailSubject = string.Join(" - ", "Invalid Format", emailMessageItem.Subject),
                                                        EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                                                        EmailTime = emailMessageItem.DateTimeReceived,
                                                        FileName = attachment.Name,
                                                        VesselName = form.VesselName
                                                    });

                                                    continue;
                                                }

                                                #region Properties Mappings

                                                vesselsRawDataDto.AttachmentId = attachment.Id;
                                                vesselsRawDataDto.ChiefEngineerName = null;
                                                vesselsRawDataDto.MasterName = null;
                                                vesselsRawDataDto.SourceFeed = "Veslinkapi (Email)";
                                                vesselsRawDataDto.FormIdentifier = form.FormIdentifier;
                                                vesselsRawDataDto.CompanyCode = form.CompanyCode;
                                                vesselsRawDataDto.CompanyName = form.CompanyName;
                                                vesselsRawDataDto.VesselCode = form.VesselCode;
                                                vesselsRawDataDto.SubmittedDate = ValidateReportData.ConvertDateTimeToUniversalTimeOrReturnNull(form.SubmittedDate); //vem-195
                                                vesselsRawDataDto.Status = form.Status;
                                                vesselsRawDataDto.ApprovedDate = ValidateReportData.ConvertDateTimeToUniversalTimeOrReturnNull(form.ApprovedDate); //vem-195
                                                vesselsRawDataDto.LatestResubmissionDate = ValidateReportData.ConvertDateTimeToUniversalTimeOrReturnNull(form.LatestResubmissionDate); //vem-195
                                                vesselsRawDataDto.FormGUID = form.FormGUID;
                                                vesselsRawDataDto.ImoNumber = form.ImoNumber;
                                                vesselsRawDataDto.UnCode = form.UnCode;

                                                vesselsRawDataDto.VesselName = form.VesselName;
                                                vesselsRawDataDto.VoyageNo = form.VoyageNo;

                                                if (!ValidateReportData.IsVesselConditionValid(form.VesselCondition)) //vem-195
                                                {
                                                    continue;
                                                }

                                                vesselsRawDataDto.VesselCondition = form.VesselCondition;
                                                vesselsRawDataDto.FWDDraft = form.FWDDraft;
                                                vesselsRawDataDto.AFTDraft = form.AFTDraft;
                                                vesselsRawDataDto.Arrival_Position = form.Arrival_Position;
                                                vesselsRawDataDto.Port = form.Port;

                                                if (!ValidateReportData.IsReportLatituteOrLongitudeValid(form.Latitude, false)) //vem-195
                                                {
                                                    continue;
                                                }
                                                vesselsRawDataDto.Latitude = form.Latitude;

                                                if (!ValidateReportData.IsReportLatituteOrLongitudeValid(form.Longitude)) //vem-195
                                                {
                                                    continue;
                                                }
                                                vesselsRawDataDto.Longitude = form.Longitude;

                                                vesselsRawDataDto.ReportTimeString = form.ReportTime;
                                                vesselsRawDataDto.ReportTime = form_reportTime;

                                                vesselsRawDataDto.Time_Drop_of_Anchor = form.Time_Drop_of_Anchor;
                                                vesselsRawDataDto.SteamingHrs = form.SteamingHrs;
                                                vesselsRawDataDto.MainEngineHrs = form.MainEngineHrs;
                                                vesselsRawDataDto.ObservedDistance = form.ObservedDistance;
                                                vesselsRawDataDto.EngineDistance = form.EngineDistance;
                                                vesselsRawDataDto.Logged_Distance = form.Logged_Distance;
                                                vesselsRawDataDto.ObsSpeed = form.ObsSpeed;
                                                vesselsRawDataDto.CPSpeed = form.CPSpeed;
                                                vesselsRawDataDto.Logged_Speed = form.Logged_Speed;
                                                vesselsRawDataDto.RPM = form.RPM;
                                                vesselsRawDataDto.MEOutputPct = form.MEOutputPct;
                                                vesselsRawDataDto.Slip = form.Slip;
                                                vesselsRawDataDto.WindForce = form.WindForce;
                                                vesselsRawDataDto.WindDirection = form.WindDirection;

                                                vesselsRawDataDto.Forob_AsOfDate = form.FOROB?.Robs?.AsOfDate;
                                                vesselsRawDataDto.Fresh_Water_Produced_since_last_report_Mts = form.Fresh_Water_Produced_since_last_report_Mts;
                                                vesselsRawDataDto.FreshWaterROB = form.FreshWaterROB;
                                                vesselsRawDataDto.Fresh_Water_for_Tank_Cleaning = form.Fresh_Water_for_Tank_Cleaning;
                                                vesselsRawDataDto.ETD = form.ETD;
                                                vesselsRawDataDto.NOR_Tendered_Time = form.NOR_Tendered_Time;
                                                vesselsRawDataDto.LOP_issued = form.LOP_issued;
                                                vesselsRawDataDto.Free_Pratique_Granted = form.Free_Pratique_Granted;
                                                vesselsRawDataDto.Etb = form.ETB;
                                                vesselsRawDataDto.Stoppages = form.Stoppages;
                                                vesselsRawDataDto.Instructed_RPM = form.Instructed_RPM;
                                                vesselsRawDataDto.Remarks = form.Remarks;
                                                vesselsRawDataDto.Distance__Vessel_Explanation = form.Distance__Vessel_Explanation;
                                                vesselsRawDataDto.Auxiliary_Engine_Instructions = form.Auxiliary_Engine_Instructions;
                                                vesselsRawDataDto.Auxilliary_Engine_Hours_Dtos = null;
                                                if (form.Auxilliary_Engine_Hours?.Auxilliary_Engine_HoursRow != null)
                                                {
                                                    List<AuxilliaryEngineHoursRow> auxilliaryEngineHoursRows = new();
                                                    if (form.Auxilliary_Engine_Hours?.Auxilliary_Engine_HoursRow.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                                    {
                                                        auxilliaryEngineHoursRows = JsonConvert.DeserializeObject<List<AuxilliaryEngineHoursRow>>(form.Auxilliary_Engine_Hours?.Auxilliary_Engine_HoursRow.ToString().Replace("\r\n", ""));
                                                    }
                                                    else
                                                    {
                                                        auxilliaryEngineHoursRows.Add(JsonConvert.DeserializeObject<AuxilliaryEngineHoursRow>(form.Auxilliary_Engine_Hours?.Auxilliary_Engine_HoursRow.ToString().Replace("\r\n", "")));
                                                    }

                                                    vesselsRawDataDto.Auxilliary_Engine_Hours_Dtos = auxilliaryEngineHoursRows.Select(x => new AuxilliaryEngineHoursDto()
                                                    {
                                                        Aux_Engine = x.Aux_Engine,
                                                        Hours = x.Hours
                                                    }).ToList();
                                                }
                                                vesselsRawDataDto.Reason_to_omit_report_from_Performance = form.Reason_to_omit_report_from_Performance;
                                                vesselsRawDataDto.SlopsROB = form.SlopsROB;
                                                vesselsRawDataDto.Slops__Annex_1_or_Annex_2 = form.Slops__Annex_1_or_Annex_2;
                                                vesselsRawDataDto.Grade_of_Slops = form.Grade_of_Slops;
                                                vesselsRawDataDto.Slop_Stowage = form.Slop_Stowage;
                                                vesselsRawDataDto.Bunker_Tank_Instructions = form.Bunker_Tank_Instructions;
                                                vesselsRawDataDto.Bunker_Rob_Instructions = form.Bunker_Rob_Instructions;
                                                vesselsRawDataDto.Reason_for_Other_Consumption1 = form.Reason_for_Other_Consumption1;
                                                vesselsRawDataDto.PSId = form.__PSId;
                                                vesselsRawDataDto.Location = form.Location;

                                                //"Logic to read the Location from POSITION REPORTS ONLY and fill the field:
                                                //< Location > N </ Location > = ""At Sea""
                                                //< Location > S </ Location > = ""In Port""
                                                //< Location > W </ Location > = ""In Port""
                                                //< Location ></ Location > = 'Drifting'
                                                //Note - for EOSP(Arrival) and COSP(Departure) reports, the location will be left blank."
                                                vesselsRawDataDto.ReportLocation = ValidateReportData.GetReportLocation(form.Location, Services.Helpers.StaticData.ReportTypeNames[form.FormIdentifier ?? ""]);

                                                vesselsRawDataDto.Event = form.Event;
                                                vesselsRawDataDto.SWBWFW = form.SWBWFW;
                                                vesselsRawDataDto.DistanceToGo = form.DistanceToGo;
                                                vesselsRawDataDto.Est_Fwd_Draft = form.Est_Fwd_Draft;
                                                vesselsRawDataDto.Est_Aft_Draft = form.Est_Aft_Draft;
                                                vesselsRawDataDto.SWBWFW_Est = form.SWBWFW_Est;
                                                vesselsRawDataDto.Total_Cargo_on_Board = form.Total_Cargo_on_Board;
                                                vesselsRawDataDto.Cargo_Grade = form.Cargo_Grade;
                                                vesselsRawDataDto.FreshWaterMade = form.FreshWaterMade;

                                                vesselsRawDataDto.BunkerStemsAndSurveysDtos = null;

                                                if (form.Bunker_Stems_and_Surveys?.Bunker_Stems_and_SurveysRow != null)
                                                {
                                                    List<BunkerStemsAndSurveysRow> bunkerStemsAndSurveysRows = new();
                                                    if (form.Bunker_Stems_and_Surveys?.Bunker_Stems_and_SurveysRow.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                                    {
                                                        bunkerStemsAndSurveysRows = JsonConvert.DeserializeObject<List<BunkerStemsAndSurveysRow>>(form.Bunker_Stems_and_Surveys?.Bunker_Stems_and_SurveysRow.ToString().Replace("\r\n", ""));
                                                    }
                                                    else
                                                    {
                                                        bunkerStemsAndSurveysRows.Add(JsonConvert.DeserializeObject<BunkerStemsAndSurveysRow>(form.Bunker_Stems_and_Surveys?.Bunker_Stems_and_SurveysRow.ToString().Replace("\r\n", "")));
                                                    }

                                                    vesselsRawDataDto.BunkerStemsAndSurveysDtos = bunkerStemsAndSurveysRows.Select(x => new BunkerStemsAndSurveysDto()
                                                    {
                                                        Number = x.Number,
                                                        Grade = x.Grade,
                                                        StemOrSurvey = x.Stem_or_Survey,
                                                        BdnFigure = x.BDN_Figure,
                                                        ShipsReceivedFigure = x.Ships_Received_Figure,
                                                        SurveyRobDifferential = x.Survey_ROB_Differential
                                                    }).ToList();
                                                }

                                                vesselsRawDataDto.Bunker_Stem_and_Survey_Description = form.Bunker_Stem_and_Survey_Description;
                                                vesselsRawDataDto.Inspections_In_Port_1 = form.Inspections_In_Port_1;
                                                vesselsRawDataDto.Inspections_In_Port_2 = form.Inspections_In_Port_2;
                                                vesselsRawDataDto.Inspections_In_Port_3 = form.Inspections_In_Port_3;
                                                vesselsRawDataDto.Inspect_Co_1 = form.Inspect_Co_1;
                                                vesselsRawDataDto.Inspect_Co_2 = form.Inspect_Co_2;
                                                vesselsRawDataDto.Inspect_Co_3 = form.Inspect_Co_3;
                                                vesselsRawDataDto.Date_1 = form.Date_1;
                                                vesselsRawDataDto.Date_2 = form.Date_2;
                                                vesselsRawDataDto.Date_3 = form.Date_3;
                                                vesselsRawDataDto.Number_of_Tugs_In = form.Number_of_Tugs_In;
                                                vesselsRawDataDto.Tug_In_Start_Time = form.Tug_In_Start_Time;
                                                vesselsRawDataDto.Number_of_Tugs_Out = form.Number_of_Tugs_Out;
                                                vesselsRawDataDto.Tugs_Out_Start_Time = form.Tugs_Out_Start_Time;
                                                vesselsRawDataDto.Number_of_Shifting_Tugs = form.Number_of_Shifting_Tugs;
                                                vesselsRawDataDto.Shifting_Tugs_Start_Time = form.Shifting_Tugs_Start_Time;
                                                vesselsRawDataDto.Berth_Shifted_To__In = form.Berth_Shifted_To__In;
                                                vesselsRawDataDto.Tugs_in_End_Time = form.Tugs_in_End_Time;
                                                vesselsRawDataDto.Berth_Shifted_From = form.Berth_Shifted_From;
                                                vesselsRawDataDto.Berth_Shifted_To = form.Berth_Shifted_To;
                                                vesselsRawDataDto.Tugs_Out_End_Time = form.Tugs_Out_End_Time;
                                                vesselsRawDataDto.Shifting_Tugs_End_Time = form.Shifting_Tugs_End_Time;
                                                vesselsRawDataDto.PEId = form.__PEId;
                                                vesselsRawDataDto.Robs = null;

                                                if (form.FOROB?.Robs?.Rob != null)
                                                {
                                                    List<RobDto> robDtos = new();
                                                    foreach (var robItem in form.FOROB?.Robs?.Rob)
                                                    {
                                                        RobDto robDto = new()
                                                        {
                                                            FuelType = ValidateReportData.FuelTypeHasValidValue(robItem.FuelType), //vem-195
                                                            Remaining = robItem.Remaining,
                                                            AuxEngineConsumption = robItem.AuxEngineConsumption,
                                                            BoilerEngineConsumption = robItem.BoilerEngineConsumption,
                                                            Units = robItem.Units,
                                                            Received = robItem.Received,
                                                            Consumption = robItem.Consumption
                                                        };

                                                        if (robItem.Allocation != null)
                                                        {
                                                            List<Allocation> allocations = new();
                                                            if (robItem.Allocation.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                                            {
                                                                allocations = JsonConvert.DeserializeObject<List<Allocation>>(robItem.Allocation.ToString().Replace("\r\n", ""));
                                                            }
                                                            else
                                                            {
                                                                allocations.Add(JsonConvert.DeserializeObject<Allocation>(robItem.Allocation.ToString().Replace("\r\n", "")));
                                                            }

                                                            robDto.Allocation = allocations.Select(x => new AllocationDto()
                                                            {
                                                                Name = x.Name,
                                                                Text = x.text
                                                            }).ToList();
                                                        }
                                                        robDtos.Add(robDto);
                                                    }
                                                    vesselsRawDataDto.Robs = robDtos;
                                                }

                                                vesselsRawDataDto.Upcomings = null;

                                                if (form.Upcoming?.UpcomingPort != null)
                                                {
                                                    List<UpcomingPort> upcomingPorts = new();

                                                    if (form.Upcoming.UpcomingPort.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                                    {
                                                        upcomingPorts = JsonConvert.DeserializeObject<List<UpcomingPort>>(form.Upcoming.UpcomingPort.ToString().Replace("\r\n", ""));
                                                    }
                                                    else
                                                    {
                                                        upcomingPorts.Add(JsonConvert.DeserializeObject<UpcomingPort>(form.Upcoming.UpcomingPort.ToString().Replace("\r\n", "")));
                                                    }

                                                    vesselsRawDataDto.Upcomings = upcomingPorts.Select(x => new UpcomingDto()
                                                    {
                                                        PortName = x.PortName,
                                                        DistToGo = x.DistToGo,
                                                        ProjSpeed = x.ProjSpeed,
                                                        Eta = x.ETA,
                                                        Via = x.Via,
                                                        UnCode = x.UnCode
                                                    }).ToList();
                                                }

                                                vesselsRawDataDto.BunkerTanks_Dtos = null;

                                                if (form.BunkerTanks?.BunkerTanksRow != null)
                                                {
                                                    List<BunkerTanksRow> bunkerTanksRows = new();

                                                    if (form.BunkerTanks?.BunkerTanksRow.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                                    {
                                                        bunkerTanksRows = JsonConvert.DeserializeObject<List<BunkerTanksRow>>(form.BunkerTanks?.BunkerTanksRow.ToString().Replace("\r\n", ""));
                                                    }
                                                    else
                                                    {
                                                        bunkerTanksRows.Add(JsonConvert.DeserializeObject<AuxilliaryEngineHoursRow>(form.BunkerTanks?.BunkerTanksRow.ToString().Replace("\r\n", "")));
                                                    }

                                                    vesselsRawDataDto.BunkerTanks_Dtos = bunkerTanksRows.Select(x => new BunkerTanksDto()
                                                    {
                                                        BunkerTankNo = x.BunkerTankNo,
                                                        BunkerTankDescription = x.BunkerTankDescription,
                                                        BunkerTankFuelGrade = x.BunkerTankFuelGrade,
                                                        BunkerTankCapacity = x.BunkerTankCapacity,
                                                        BunkerTankObservedVolume = x.BunkerTankObservedVolume,
                                                        BunkerTankUnit = x.BunkerTankUnit,
                                                        BunkerTankROB = x.BunkerTankROB,
                                                        BunkerTankFillPercent = x.BunkerTankFillPercent,
                                                        BunkerTankSupplyDate = x.BunkerTankSupplyDate
                                                    }).ToList();
                                                }

                                                if (form.Cargo?.Cargoes != null)
                                                {
                                                    var cargoList = new List<Cargo>
                          {
                            JsonConvert.DeserializeObject<Cargo>(form.Cargo?.Cargoes?.Cargo.ToString().Replace("\r\n", ""))
                          };

                                                    var CargoHandlingDtoList = new List<CargoHandlingDto>();

                                                    foreach (var cargoObj in cargoList)
                                                    {
                                                        var CargoHandlingDtoObj = new CargoHandlingDto();
                                                        var stowageDtoList = new List<StowageDto>();

                                                        CargoHandlingDtoObj.CargoTypeID = cargoObj.CargoTypeID;
                                                        CargoHandlingDtoObj.Function = cargoObj.Function;
                                                        CargoHandlingDtoObj.Berth = cargoObj.Berth;
                                                        CargoHandlingDtoObj.BLDate = cargoObj.BLDate;
                                                        CargoHandlingDtoObj.BLGross = cargoObj.BLGross;
                                                        CargoHandlingDtoObj.BLCode = cargoObj.BLCode;
                                                        CargoHandlingDtoObj.CargoName = cargoObj.CargoName;
                                                        CargoHandlingDtoObj.ShipGross = cargoObj.ShipGross;
                                                        CargoHandlingDtoObj.LoadTemp = cargoObj.LoadTemp;
                                                        CargoHandlingDtoObj.APIGravity = cargoObj.APIGravity;
                                                        CargoHandlingDtoObj.UnitCode = cargoObj.UnitCode;
                                                        CargoHandlingDtoObj.Charterer = cargoObj.Charterer;
                                                        CargoHandlingDtoObj.Consignee = cargoObj.Consignee;
                                                        CargoHandlingDtoObj.Receiver = cargoObj.Receiver;
                                                        CargoHandlingDtoObj.Shipper = cargoObj.Shipper;
                                                        CargoHandlingDtoObj.Destination = cargoObj.Destination;
                                                        CargoHandlingDtoObj.LetterOfProtest = cargoObj.LetterOfProtest;

                                                        if (cargoObj.Stowage != null)
                                                        {
                                                            foreach (var stowageObj in cargoObj.Stowage)
                                                            {
                                                                var stowageDto = new StowageDto
                                                                {
                                                                    TankName = stowageObj.TankName,
                                                                    Quantity = stowageObj.Quantity,
                                                                    Unit = stowageObj.Unit
                                                                };
                                                                stowageDtoList.Add(stowageDto);
                                                            }
                                                            CargoHandlingDtoObj.Stowage = stowageDtoList;
                                                        }
                                                        CargoHandlingDtoList.Add(CargoHandlingDtoObj);
                                                    }

                                                    vesselsRawDataDto.CargoHandling = CargoHandlingDtoList;
                                                }
                                                if (form.PortActivities?.Activity != null)
                                                {
                                                    var ActivityLists = new List<Activity>();
                                                    ActivityLists = JsonConvert.DeserializeObject<List<Activity>>(form.PortActivities.Activity.ToString().Replace("\r\n", ""));

                                                    var PortActivityDtoList = new List<PortActivitiesDto>();

                                                    foreach (var activity in ActivityLists)
                                                    {
                                                        var portActivityDto = new PortActivitiesDto
                                                        {
                                                            ActivityName = activity.ActivityName,
                                                            Time = activity.Time,
                                                            CargoName = activity.CargoName,
                                                            Charterer = activity.Charterer,
                                                            Remarks = activity.Remarks,
                                                            Berth = activity.Berth
                                                        };
                                                        PortActivityDtoList.Add(portActivityDto);
                                                    }
                                                    vesselsRawDataDto.PortActivities = PortActivityDtoList;
                                                }

                                                vesselsRawDataDto.GmtOffset = form.GmtOffset;

                                                #endregion

                                                var vesselRawDataId = await _serviceManager.VesselRawDataService.CreateAsync(vesselsRawDataDto);

                                                var NdrApiUrl = _configuration["API:NdrApiUrl"] + vesselRawDataId;
                                                var NdrClient = new RestClient(NdrApiUrl);
                                                var NdrRequest = new RestRequest(NdrApiUrl, Method.Post);
                                                await NdrClient.ExecutePostAsync(NdrRequest);

                                                Thread.Sleep(5000);

                                                DbAttachmentIds.Add(attachment.Id);

                                                index++;
                                            }
                                        }
                                    }
                                }

                                if (emailMessageItem != null)
                                    successfullyCompletedEmails.Add((EmailMessage)emailMessageItem);
                            }
                            catch (Exception ex)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.Append("Error - " + DateTime.UtcNow.ToLongDateString());
                                stringBuilder.Append(Environment.NewLine);
                                stringBuilder.Append("Source : " + ex.Source);
                                stringBuilder.Append(Environment.NewLine);
                                stringBuilder.Append("Error Exception : " + ex.Message);
                                stringBuilder.Append(Environment.NewLine);
                                stringBuilder.Append("Inner Exception : " + ex.InnerException?.Message ?? "");
                                stringBuilder.Append(Environment.NewLine);

                                Log.Information(stringBuilder.ToString());
                                Log.Error(ex, "Error Stacktrace");

                                Log.Information(Environment.NewLine);
                                Log.Information(Environment.NewLine);
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Information(Environment.NewLine);
                            Log.Information("Vessel Email Import Error");
                            Log.Information(Environment.NewLine);
                            Log.Information("Exception : " + ex.Message + (ex.InnerException != null ? " InnerException : " + ex.InnerException.Message : ""));
                            Log.Information(Environment.NewLine);
                        }
                    }
                    else
                    {
                        // Mail is being marked as read even though it is invalid, because, next time, if we take the descending orderwise batch, then this invalid mail won't be coming.
                        successfullyCompletedEmails.Add((EmailMessage)emailMessageItem);

                        await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                        {
                            ServiceName = vesselsXMLName,
                            Description = "The received email is not having expected subject line.",
                            EmailSubject = string.Join(" - ", "Unknown Email", emailMessageItem.Subject),
                            EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                            EmailTime = emailMessageItem.DateTimeReceived
                        });
                    }
                }

                Log.Information($"Till now {index} mails have been read.");
                Log.Information($"XML Reading Service Ended at: {DateTime.Now}");

                if (successfullyCompletedEmails.Count > 0)
                {
                    successfullyCompletedEmails.ForEach(x => x.IsRead = true);
                    ewsClient.UpdateItems(successfullyCompletedEmails, inbox.Id, ConflictResolutionMode.AutoResolve, MessageDisposition.SaveOnly, null);
                }
            }

            return Ok(new CommonApiResponse()
            {
                data = new(),
                message = "Reports are imported",
                statusCode = (int)HttpStatusCode.OK
            });
        }

        [HttpGet]
        [Route("ExcelData")]
        public async Task<IActionResult> GetExcelData()
        {
            //    Using Microsoft.Identity.Client 4.22.0
            var cca = ConfidentialClientApplicationBuilder
                        .Create(_configuration["MailConfiguration:AppId"])
                        .WithClientSecret(_configuration["MailConfiguration:ClientSecret"])
                        .WithTenantId(_configuration["MailConfiguration:TenantId"])
                        .Build();

            var ewsScopes = new string[] { "https://outlook.office365.com/.default" };

            var authResult = await cca.AcquireTokenForClient(ewsScopes).ExecuteAsync();

            // Configure the ExchangeService with the access token
            var ewsClient = new ExchangeService
            {
                Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx"),
                Credentials = new OAuthCredentials(authResult.AccessToken),
                ImpersonatedUserId =
                new ImpersonatedUserId(ConnectingIdType.SmtpAddress, "vrpts@ship-watch.com")
            };

            //Include x-anchormailbox header
            ewsClient.HttpHeaders.Add("X-AnchorMailbox", "vrpts@ship-watch.com");

            // Make an EWS call
            var folders = ewsClient.FindFolders(WellKnownFolderName.MsgFolderRoot, new FolderView(10));

            Folder inbox = Folder.Bind(ewsClient, WellKnownFolderName.Inbox);
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
            ItemView view = new(20);

            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);

            bool isMoreItemAvailable = false;

            List<string?> DbAttachmentIds = await _serviceManager.VesselRawDataService.GetAllExistedAttachmentIdAsync();

            var findResults = ewsClient.FindItems(WellKnownFolderName.Inbox, sf, view);
            isMoreItemAvailable = findResults.MoreAvailable;

            List<EmailMessage?> emailslist = new(findResults.TotalCount);
            if (findResults.TotalCount > 0)
            {
                var _unreadEmailslist = findResults.Items.Where(x => x != null).Select(x => x as EmailMessage).ToList();
                emailslist = _unreadEmailslist;
                try
                {
                    ewsClient.LoadPropertiesForItems(emailslist, PropertySet.FirstClassProperties);
                }
                catch (Exception ex)
                {
                    await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                    {
                        ServiceName = vesselsExcelName,
                        StartedDateTime = _serviceStartedTime,
                        EndedDateTime = _serviceEndedTime,
                        ResourceUri = ewsClient.Url,
                        RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                        ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)ex).Response),
                        RecordsToBeProcessed = 0,
                        RecordsActuallyProcessed = 0,
                        Content = null,
                        StatusCode = ((int)((ServiceResponseException)ex).ErrorCode).ToString(),
                        StatusDescription = ex.Message,
                        Source = ex?.Source,
                        Message = ex?.Message,
                        InnerException = ex?.InnerException?.ToString(),
                        StackTrace = ex?.StackTrace
                    });

                    try
                    {
                        Thread.Sleep(5000);
                        ewsClient.LoadPropertiesForItems(emailslist, PropertySet.FirstClassProperties);
                    }
                    catch (Exception exAfter5Seconds)
                    {
                        await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                        {
                            ServiceName = vesselsExcelName,
                            StartedDateTime = _serviceStartedTime,
                            EndedDateTime = _serviceEndedTime,
                            ResourceUri = ewsClient.Url,
                            RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                            ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)exAfter5Seconds).Response),
                            RecordsToBeProcessed = 0,
                            RecordsActuallyProcessed = 0,
                            Content = null,
                            StatusCode = ((int)((ServiceResponseException)exAfter5Seconds).ErrorCode).ToString(),
                            StatusDescription = exAfter5Seconds.Message,
                            Source = exAfter5Seconds?.Source,
                            Message = exAfter5Seconds?.Message,
                            InnerException = exAfter5Seconds?.InnerException?.ToString(),
                            StackTrace = exAfter5Seconds?.StackTrace
                        });
                    }

                    return BadRequest(new CommonApiResponse()
                    {
                        data = new(),
                        message = ex?.Message,
                        statusCode = (int)HttpStatusCode.BadRequest
                    });
                }

                var successfullyCompletedEmails = new List<EmailMessage>();

                Log.Information($"Total {findResults.Items.Count} mails to be read.");

                int index = 0;

                foreach (var emailMessageItem in findResults.Items)
                {
                    #region Attachments

                    try
                    {
                        try
                        {
                            EmailMessage.Bind(ewsClient, emailMessageItem.Id, new PropertySet(ItemSchema.Attachments));
                        }
                        catch (Exception ex)
                        {
                            await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                            {
                                ServiceName = vesselsExcelName,
                                StartedDateTime = _serviceStartedTime,
                                EndedDateTime = _serviceEndedTime,
                                ResourceUri = ewsClient.Url,
                                RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                                ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)ex).Response),
                                RecordsToBeProcessed = 0,
                                RecordsActuallyProcessed = 0,
                                Content = null,
                                StatusCode = ((int)((ServiceResponseException)ex).ErrorCode).ToString(),
                                StatusDescription = ex.Message,
                                Source = ex?.Source,
                                Message = ex?.Message,
                                InnerException = ex?.InnerException?.ToString(),
                                StackTrace = ex?.StackTrace
                            });

                            try
                            {
                                Thread.Sleep(5000);
                                EmailMessage.Bind(ewsClient, emailMessageItem.Id, new PropertySet(ItemSchema.Attachments));
                            }
                            catch (Exception exAfter5Seconds)
                            {
                                await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                                {
                                    ServiceName = vesselsExcelName,
                                    StartedDateTime = _serviceStartedTime,
                                    EndedDateTime = _serviceEndedTime,
                                    ResourceUri = ewsClient.Url,
                                    RequestJSONBody = JsonConvert.SerializeObject(ewsClient),
                                    ResponseJSONBody = JsonConvert.SerializeObject(((ServiceResponseException)exAfter5Seconds).Response),
                                    RecordsToBeProcessed = 0,
                                    RecordsActuallyProcessed = 0,
                                    Content = null,
                                    StatusCode = ((int)((ServiceResponseException)exAfter5Seconds).ErrorCode).ToString(),
                                    StatusDescription = exAfter5Seconds.Message,
                                    Source = exAfter5Seconds?.Source,
                                    Message = exAfter5Seconds?.Message,
                                    InnerException = exAfter5Seconds?.InnerException?.ToString(),
                                    StackTrace = exAfter5Seconds?.StackTrace
                                });
                            }
                        }

                        foreach (Microsoft.Exchange.WebServices.Data.Attachment attachment in emailMessageItem.Attachments)
                        {
                            if ((attachment.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && attachment.ContentType != "application/octet-stream") ||
                                (DbAttachmentIds != null && DbAttachmentIds.Count > 0 && DbAttachmentIds.Contains(attachment.Id)))
                            {
                                await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                                {
                                    ServiceName = vesselsExcelName,
                                    Description = attachment.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && attachment.ContentType != "application/octet-stream" ?
                                                                            "The received email is not having expected file." : "This email attachment report file was already read",
                                    EmailSubject = string.Join(" - ", "Invalid File", emailMessageItem.Subject),
                                    EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                                    EmailTime = emailMessageItem.DateTimeReceived,
                                    FileName = attachment.Name
                                });

                                continue;
                            }


                            if (attachment is FileAttachment _attachment)
                            {
                                //var _attachment = (FileAttachment)attachment;

                                _attachment.Load();

                                if (_attachment.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || _attachment.ContentType == "application/octet-stream")
                                {
                                    var stream = new MemoryStream(_attachment.Content);

                                    // Generate a unique temporary file name
                                    string tempFileName = "../TempReportFile.xlsx";

                                    // Set the full path of the temporary file
                                    FileStream file = new(tempFileName, FileMode.Create, FileAccess.Write);

                                    stream.WriteTo(file);
                                    file.Close();

                                    // Open the Excel file using ClosedXML
                                    using var workbook = new XLWorkbook(file.Name);
                                    // Get the first worksheet from the Excel file
                                    var worksheet = workbook.Worksheets.Where(x => x.Worksheet.Name == "Table").FirstOrDefault();

                                    // Create a dictionary to store the data
                                    Dictionary<string, string?> data = new();
                                    if (worksheet != null)
                                    {
                                        var firstRow = worksheet.Row(1);
                                        var secondRow = worksheet.Row(2);

                                        // Iterate over each cell in the first row and store the corresponding value from the second row in the dictionary
                                        foreach (var cell in firstRow.CellsUsed())
                                        {
                                            var cellValue = cell.Value;
                                            if (!String.IsNullOrEmpty(cellValue.ToString()))
                                            {

                                                string key = cellValue.ToString();

                                                // Find the corresponding cell in the second row
                                                var correspondingCell = secondRow.Cell(cell.Address.ColumnNumber);

                                                // Get the value from the corresponding cell in the second row
                                                var correspondingCellValue = correspondingCell.Value;
                                                string value = correspondingCellValue.ToString();

                                                // Add the key-value pair to the dictionary
                                                data.Add(key, value);

                                            }
                                        }
                                        var KeyList = data.Keys.ToList();
                                        var containsAllKeys = KeyList.Select(x => x).Except(ExcelFieldData.ExcelReportField.Values).ToList();
                                        if (containsAllKeys.Count > 0)
                                        {
                                            await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                                            {
                                                ServiceName = vesselsExcelName,
                                                Description = "The received email is having new fields.",
                                                EmailSubject = string.Join(" - ", "New Filed Received", emailMessageItem.Subject),
                                                EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                                                EmailTime = emailMessageItem.DateTimeReceived,
                                                FileName = attachment.Name,
                                                VesselName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselName])
                                            });
                                        }

                                        var LatitudeDeg = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LatitudeDeg]);
                                        var LatitudeMin = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LatitudeMin]);
                                        var LatitudeSec = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LatitudeSec]);
                                        var LatitudeDir = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LatitudeDir]);

                                        var LatDeg = !string.IsNullOrEmpty(LatitudeDeg) ? LatitudeDeg : "0";
                                        var LatMin = !string.IsNullOrEmpty(LatitudeMin) ? LatitudeMin : "0";
                                        var LatSec = !string.IsNullOrEmpty(LatitudeSec) ? LatitudeSec : "0";
                                        var LatDir = !string.IsNullOrEmpty(LatitudeDir) ? LatitudeDir : null;

                                        var LongitudeDeg = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LongitudeDeg]);
                                        var LongitudeMin = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LongitudeMin]);
                                        var LongitudeSec = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LongitudeSec]);
                                        var LongitudeDir = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LongitudeDir]);

                                        var LongDeg = !string.IsNullOrEmpty(LongitudeDeg) ? LongitudeDeg : "0";
                                        var LongMin = !string.IsNullOrEmpty(LongitudeMin) ? LongitudeMin : "0";
                                        var LongSec = !string.IsNullOrEmpty(LongitudeSec) ? LongitudeSec : "0";
                                        var LongDir = !string.IsNullOrEmpty(LongitudeDir) ? LongitudeDir : null;

                                        var Latitude = $"{LatDeg} {LatMin}' {LatSec}\" {LatDir}";
                                        var Longitude = $"{LongDeg} {LongMin}' {LongSec}\" {LongDir}";
                                        // var Longitude = $"{data.GetValueOrDefault("LongitudeDeg")} {data.GetValueOrDefault("LongitudeMin")}' {data.GetValueOrDefault("LongitudeSec")}\" {data.GetValueOrDefault("LongitudeDir")}";

                                        var DatetimefromExcel = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Date])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Date])) : 0;

                                        if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.Time]))
                                        {
                                            DatetimefromExcel += !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Time])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Time])) : 0;
                                        }
                                        else if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.LocalTime]))
                                        {
                                            DatetimefromExcel += !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LocalTime])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LocalTime])) : 0;
                                        }

                                        var convertedDate = DateTime.FromOADate(DatetimefromExcel);
                                        var convertedDateString = convertedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                        var UTCoffSet = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCOffset]);
                                        if (UTCoffSet == "00:00")
                                        {
                                            UTCoffSet = "+00:00";
                                        }
                                        var dateAndOffset = convertedDateString + UTCoffSet;

                                        string? portValue = null;
                                        if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.CurrentPort]))
                                        {
                                            portValue = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.CurrentPort]);
                                        }
                                        else if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.ArrivalPort]))
                                        {
                                            portValue = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ArrivalPort]);
                                        }
                                        else if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.DeparturePort]))
                                        {
                                            portValue = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DeparturePort]);
                                        }

                                        double parseDateFromExcel = 0;
                                        bool isUtcTimestampExist = true;

                                        if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.UTC_Timestamp]) && ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTC_Timestamp])))
                                        {
                                            isUtcTimestampExist = string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTC_Timestamp]));
                                            parseDateFromExcel = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTC_Timestamp])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTC_Timestamp])) : 0;
                                        }
                                        else if (data.ContainsKey(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCTimestamp]) && ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCTimestamp])))
                                        {
                                            isUtcTimestampExist = string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCTimestamp]));
                                            parseDateFromExcel = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCTimestamp])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UTCTimestamp])) : 0;
                                        }
                                        var UTCDate = parseDateFromExcel != 0 ? DateTime.FromOADate(parseDateFromExcel).ToString("yyyy-MM-ddTHH:mm:ss") : null;

                                        if (DatetimefromExcel == 0 ||
                                            string.IsNullOrEmpty(Latitude) ||
                                            string.IsNullOrEmpty(Longitude) ||
                                            (string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselIMO])) &&
                                            ValidateReportData.ValidateData("Integer", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselIMO]))) ||
                                            (isUtcTimestampExist &&
                                            string.IsNullOrEmpty(dateAndOffset)) ||
                                            string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType])))
                                        {
                                            await _serviceManager.ServiceLogsService.CreateServiceNotificationAsync(new TbServiceNotifications()
                                            {
                                                ServiceName = vesselsExcelName,
                                                Description = "The received email is not having expected Format.",
                                                EmailSubject = string.Join(" - ", "Invalid Format", emailMessageItem.Subject),
                                                EmailReceivedFrom = ((EmailMessage)emailMessageItem).From.Address,
                                                EmailTime = emailMessageItem.DateTimeReceived,
                                                FileName = attachment.Name,
                                                VesselName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselName])
                                            });
                                            continue;
                                        }

                                        var RobList = new List<RobDto>();



                                        var allocationLGOList = new List<AllocationDto>();
                                        var allocationLFOList = new List<AllocationDto>();
                                        var allocationIFOList = new List<AllocationDto>();
                                        var allocationMGOList = new List<AllocationDto>();

                                        foreach (FuelTypes fuel in Enum.GetValues(typeof(FuelTypes)))
                                        {
                                            RobDto robDto = new()
                                            {
                                                FuelType = fuel.ToString()
                                            };
                                            RobList.Add(robDto);
                                        }

                                        foreach (var key in KeyList)
                                        {

                                            if (key.Contains(FuelTypes.LGO.ToString()))
                                            {
                                                if (data.GetValueOrDefault(key) != "0")
                                                {
                                                    if (key == ExcelReportFields.AETotalConsumption_LGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LGO.ToString()).Select(x => { x.AuxEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.BoilerTotalConsumption_LGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LGO.ToString()).Select(x => { x.BoilerEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.TotalConsumption_LGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LGO.ToString()).Select(x => { x.Consumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.ROB_LGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LGO.ToString()).Select(x => { x.Remaining = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else
                                                    {
                                                        var allocationObj = new AllocationDto
                                                        {
                                                            Name = key.Replace("_LGO", ""),
                                                            Text = data.GetValueOrDefault(key)
                                                        };
                                                        allocationLGOList.Add(allocationObj);

                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LGO.ToString()).Select(x => { x.Allocation = allocationLGOList; return x; }).ToList();
                                                    }
                                                }
                                            }
                                            else if (key.Contains(FuelTypes.LFO.ToString()))
                                            {
                                                if (data.GetValueOrDefault(key) != "0")
                                                {
                                                    if (key == ExcelReportFields.AETotalConsumption_LFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LFO.ToString()).Select(x => { x.AuxEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.BoilerTotalConsumption_LFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LFO.ToString()).Select(x => { x.BoilerEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();

                                                    }
                                                    else if (key == ExcelReportFields.TotalConsumption_LFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LFO.ToString()).Select(x => { x.Consumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();

                                                    }
                                                    else if (key == ExcelReportFields.ROB_LFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LFO.ToString()).Select(x => { x.Remaining = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else
                                                    {
                                                        var allocationObj = new AllocationDto
                                                        {
                                                            Name = key.Replace("_LFO", ""),
                                                            Text = data.GetValueOrDefault(key)
                                                        };
                                                        allocationLFOList.Add(allocationObj);
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.LFO.ToString()).Select(x => { x.Allocation = allocationLFOList; return x; }).ToList();

                                                    }
                                                }
                                            }
                                            else if (key.Contains(FuelTypes.MGO.ToString()))
                                            {
                                                if (data.GetValueOrDefault(key) != "0")
                                                {

                                                    if (key == ExcelReportFields.AETotalConsumption_MGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.MGO.ToString()).Select(x => { x.AuxEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.BoilerTotalConsumption_MGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.MGO.ToString()).Select(x => { x.BoilerEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.TotalConsumption_MGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.MGO.ToString()).Select(x => { x.Consumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.ROB_MGO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.MGO.ToString()).Select(x => { x.Remaining = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else
                                                    {
                                                        var allocationObj = new AllocationDto
                                                        {
                                                            Name = key.Replace("_MGO", ""),
                                                            Text = data.GetValueOrDefault(key)
                                                        };
                                                        allocationMGOList.Add(allocationObj);

                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.MGO.ToString()).Select(x => { x.Allocation = allocationMGOList; return x; }).ToList();

                                                    }
                                                }
                                            }
                                            else if (key.Contains(FuelTypes.IFO.ToString()))
                                            {
                                                if (data.GetValueOrDefault(key) != "0")
                                                {
                                                    if (key == ExcelReportFields.AETotalConsumption_IFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.IFO.ToString()).Select(x => { x.AuxEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.BoilerTotalConsumption_IFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.IFO.ToString()).Select(x => { x.BoilerEngineConsumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.TotalConsumption_IFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.IFO.ToString()).Select(x => { x.Consumption = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else if (key == ExcelReportFields.ROB_IFO.ToString())
                                                    {
                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.IFO.ToString()).Select(x => { x.Remaining = Convert.ToDecimal(data.GetValueOrDefault(key)); return x; }).ToList();
                                                    }
                                                    else
                                                    {
                                                        var allocationObj = new AllocationDto
                                                        {
                                                            Name = key.Replace("_IFO", ""),
                                                            Text = data.GetValueOrDefault(key)
                                                        };
                                                        allocationIFOList.Add(allocationObj);

                                                        _ = RobList.Where(x => x.FuelType == FuelTypes.IFO.ToString()).Select(x => { x.Allocation = allocationIFOList; return x; }).ToList();

                                                    }
                                                }
                                            }
                                        }
                                        var ETDfromExcel = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCTimestamp])) && !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCTimestamp])) ? Convert.ToDouble(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCTimestamp])) : 0;

                                        //var ETDfromExcel = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCTimestamp])) ? Convert.ToDouble(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCTimestamp])) : 0;
                                        var ETDconvertedDate = ETDfromExcel != 0 ? DateTime.FromOADate(ETDfromExcel).ToString("yyyy-MM-ddTHH:mm:ss") + data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETDUTCOffset]) : null;
                                        var VesselRawObj = new VesselsRawDataDto();

                                        VesselRawObj.AFTDraft = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DraftAftm])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DraftAftm])) : null;
                                        VesselRawObj.FWDDraft = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DraftForem])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DraftForem])) : null;
                                        VesselRawObj.Remarks = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.GeneralRemarks]);
                                        VesselRawObj.VesselCondition = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselConditionBallastLaden]);
                                        VesselRawObj.SourceFeed = "Email Excel";
                                        VesselRawObj.Latitude = Latitude;
                                        VesselRawObj.Longitude = Longitude;
                                        VesselRawObj.ChiefEngineerName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ChiefEngineer]);
                                        VesselRawObj.MasterName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Master]);
                                        VesselRawObj.ImoNumber = ValidateReportData.ValidateData("Integer", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselIMO])) ? data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselIMO]) : null;
                                        VesselRawObj.VesselName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.VesselName]);
                                        var reportType = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]) == "At Sea" || data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]) == "In Port" ? "Position" : data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]);
                                        VesselRawObj.ReportType = reportType;
                                        VesselRawObj.Port = portValue;

                                        var DateString = !string.IsNullOrEmpty(UTCDate) ? UTCDate : convertedDateString;

                                        VesselRawObj.ReportTime = ValidateReportData.GetDateTimeFromOffset(DateString + UTCoffSet);

                                        VesselRawObj.ReportTimeString = DateString + UTCoffSet;

                                        VesselRawObj.ETD = ETDconvertedDate;

                                        VesselRawObj.RPM = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.AverageRPM])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.AverageRPM])) : null;

                                        VesselRawObj.WindForce = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.BeaufortForce])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.BeaufortForce])) : null;

                                        VesselRawObj.CPSpeed = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.InstructedSpeedkn])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.InstructedSpeedkn])) : null;
                                        VesselRawObj.Instructed_RPM = (data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.InstructedRPM]));

                                        VesselRawObj.Logged_Distance = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LoggedDistancenm])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.LoggedDistancenm])) : null;

                                        VesselRawObj.MEPowerkW = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.MEPowerkW])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.MEPowerkW])) : null;

                                        VesselRawObj.ObservedDistance = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ObservedDistancenm])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ObservedDistancenm])) : null;

                                        VesselRawObj.Slip = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Slip])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.Slip])) : null;

                                        VesselRawObj.ObsSpeed = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SpeedOverGroundkn])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SpeedOverGroundkn])) : null;

                                        VesselRawObj.Logged_Speed = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SpeedThroughWaterkn])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SpeedThroughWaterkn])) : null;

                                        VesselRawObj.SteamingHrs = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SteamingHoursSinceLastReport])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.SteamingHoursSinceLastReport])) : null;

                                        VesselRawObj.ManouveringHours = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ManouveringHours])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ManouveringHours])) : null;

                                        VesselRawObj.DSS = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DSS])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DSS])) : null;

                                        VesselRawObj.HoursSinceLastReport = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.HoursSinceLastReport])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.HoursSinceLastReport])) : null;

                                        VesselRawObj.StoppageRemarks = !string.IsNullOrWhiteSpace(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.StoppageRemarks])) ? data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.StoppageRemarks]) : null;

                                        VesselRawObj.Stoppages = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.StoppageHours]) ?? null;
                                        VesselRawObj.WindDirection = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.WindDirection]) ?? null;

                                        VesselRawObj.CargoLoad = !string.IsNullOrWhiteSpace(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.CargoLoad])) ? data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.CargoLoad]) : null;

                                        VesselRawObj.CargoDischarge = !string.IsNullOrWhiteSpace(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.CargoDischarge])) ? data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.CargoDischarge]) : null;

                                        VesselRawObj.RemarksforOtherCons = !string.IsNullOrWhiteSpace(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.RemarksforOtherCons])) ? data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.RemarksforOtherCons]) : null;

                                        VesselRawObj.Total_Cargo_on_Board = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.TotalCargoOnBoard])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.TotalCargoOnBoard])) : null;

                                        if (data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]) == "At Sea")
                                        {
                                            VesselRawObj.Location = "N";
                                            VesselRawObj.ReportLocation = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]);
                                        }
                                        else if (data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]) == "In Port")
                                        {
                                            VesselRawObj.Location = "S";
                                            VesselRawObj.ReportLocation = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ReportType]);
                                        }

                                        List<UpcomingDto> upcomingPorts = new();

                                        var ETATimestamp = !string.IsNullOrEmpty(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETATimestampUTC])) ? double.Parse(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETATimestampUTC])) : 0;

                                        var ETATimestampUTC = ETATimestamp != 0 ? DateTime.FromOADate(ETATimestamp).ToString("yyyy-MM-ddTHH:mm:ss") : null;
                                        var upcommingObj = new UpcomingDto
                                        {
                                            DistToGo = ValidateReportData.ValidateData("Decimal", data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DistancetoDestination])) ? Convert.ToDecimal(data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.DistancetoDestination])) : null,
                                            PortName = data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.UpcomingPort]),
                                            Eta = ETATimestampUTC + data.GetValueOrDefault(ExcelFieldData.ExcelReportField[ExcelReportFields.ETAUTCOffset])
                                        };
                                        upcomingPorts.Add(upcommingObj);
                                        VesselRawObj.Upcomings = upcomingPorts;
                                        VesselRawObj.Robs = RobList;

                                        await _serviceManager.VesselRawDataService.CreateAsync(VesselRawObj);
                                        index++;
                                    }

                                    stream.Close();
                                    System.IO.File.Delete(tempFileName);
                                }
                            }
                        }

                        if (emailMessageItem != null)
                            successfullyCompletedEmails.Add((EmailMessage)emailMessageItem);
                    }
                    catch (Exception ex)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Error - " + DateTime.UtcNow.ToLongDateString());
                        stringBuilder.Append(Environment.NewLine);
                        stringBuilder.Append("Source : " + ex.Source);
                        stringBuilder.Append(Environment.NewLine);
                        stringBuilder.Append("Error Exception : " + ex.Message);
                        stringBuilder.Append(Environment.NewLine);
                        stringBuilder.Append("Inner Exception : " + ex.InnerException?.Message ?? "");
                        stringBuilder.Append(Environment.NewLine);

                        Log.Information(stringBuilder.ToString());
                        Log.Error(ex, "Error Stacktrace");

                        Log.Information(Environment.NewLine);
                        Log.Information(Environment.NewLine);
                    }

                    #endregion
                }

                Log.Information($"Till now {index} mails have been read.");
                Log.Information($"Excel Reading Service Ended at: {DateTime.Now}");

                if (successfullyCompletedEmails.Count > 0)
                {
                    successfullyCompletedEmails.ForEach(x => x.IsRead = true);
                    ewsClient.UpdateItems(successfullyCompletedEmails, inbox.Id, ConflictResolutionMode.AutoResolve, MessageDisposition.SaveOnly, null);
                }
            }
            return Ok(new CommonApiResponse()
            {
                data = new(),
                message = "Reports are imported",
                statusCode = (int)HttpStatusCode.OK
            });
        }
    }
}
