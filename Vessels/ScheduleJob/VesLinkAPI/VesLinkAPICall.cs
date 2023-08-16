using DataTransferObjects.VesselsRawData;
using Entities.Models;
using Mapster;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using ServiceContracts;
using Services.Helpers;
using System.Xml;
using Vessels.ApiModels;
using static Vessels.ApiModels.VessLinkModel;

namespace Vessels.ScheduleJob.VesLinkAPI
{
  public class VesLinkAPICall : CronJobService
  {
    private readonly IServiceProvider _provider;

    public VesLinkAPICall(IScheduleConfig<VesLinkAPICall> config, IServiceProvider provider)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
      _provider = provider;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
      return base.StartAsync(cancellationToken);
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
      try
      {
        Log.Information($"Veslink API Call Starts at: {DateTime.Now}");

        DateTime startedDateTime = DateTime.UtcNow;
        var _serviceManager = _provider.CreateScope().ServiceProvider.GetService<IServiceManager>();

        dynamic ResponseList = new List<dynamic>();
        dynamic XMLList = new List<dynamic>();
        var resData = _serviceManager.VesselRawDataService.GetData();
        if (resData.Item1 != null && resData.Item1.Count > 0)
        {
          int rawDataIndex = 0;

          Log.Information($"Total {resData.Item1.Count} records have returned from vessel raw data service");

          foreach (var rawData in resData.Item1)
          {
            var VesselObj = resData.Item2.Where(x => x.Id == rawData.VesselRawDataId).FirstOrDefault();

            var FormData = new VessLinkReportForm
            {
              FormIdentifier = rawData.FormIdentifier,
              CompanyCode = rawData.CompanyCode,
              CompanyName = rawData.CompanyName,
              VesselCode = rawData.VesselCode,
              SubmittedDate = rawData.SubmittedDate,
              Status = rawData.Status,
              ApprovedDate = rawData.ApprovedDate,
              LatestResubmissionDate = rawData.LatestResubmissionDate,
              FormGUID = rawData.FormGUID,
              ImoNumber = rawData.ImoNumber,
              VesselName = rawData.VesselName,
              ReportTime = rawData.ReportTimeString,
              VoyageNo = rawData.VoyageNo,
              FWDDraft = rawData.FWDDraft,
              AFTDraft = rawData.AFTDraft,
              Port = rawData.Port,
              ETD = rawData.ETD,
              Latitude = rawData.Latitude,
              Longitude = rawData.Longitude
            };

            if (rawData.Upcomings != null && rawData.Upcomings.Count > 0)
            {
              var upcomingPortList = new Upcoming_Ports();
              foreach (var upcommingPortObj in rawData.Upcomings)
              {
                var upcomming = new UpcomingPort
                {
                  PortName = upcommingPortObj.PortName,
                  DistToGo = upcommingPortObj.DistToGo,
                  ProjSpeed = upcommingPortObj.ProjSpeed,
                  ETA = upcommingPortObj.Eta,
                  Via = upcommingPortObj.Via,
                  UnCode = upcommingPortObj.UnCode
                };

                upcomingPortList.UpcomingPort = upcomming;
              }
              FormData.Upcoming = upcomingPortList;
            }
            FormData.SteamingHrs = rawData.SteamingHrs;
            FormData.MainEngineHrs = rawData.MainEngineHrs;
            FormData.Stoppages = rawData.Stoppages;
            FormData.Instructed_RPM = rawData.Instructed_RPM;
            FormData.ObservedDistance = rawData.ObservedDistance;
            FormData.EngineDistance = rawData.EngineDistance;
            FormData.Logged_Distance = rawData.Logged_Distance;
            FormData.Remarks = rawData.Remarks;
            FormData.ObsSpeed = rawData.ObsSpeed;
            FormData.CPSpeed = rawData.CPSpeed;
            FormData.Logged_Speed = rawData.Logged_Speed;
            FormData.Distance__Vessel_Explanation = rawData.Distance__Vessel_Explanation;
            FormData.RPM = rawData.RPM;
            FormData.MEOutputPct = rawData.MEOutputPct;
            FormData.Slip = rawData.Slip;
            FormData.Auxiliary_Engine_Instructions = rawData.Auxiliary_Engine_Instructions;
            FormData.WindDirection = rawData.WindDirection;
            FormData.WindForce = rawData.WindForce;
            FormData.Reason_to_omit_report_from_Performance = rawData.Reason_to_omit_report_from_Performance;
            FormData.Bunker_Stem_and_Survey_Description = rawData.Bunker_Stem_and_Survey_Description;
            FormData.Bunker_Tank_Instructions = rawData.Bunker_Tank_Instructions;
            FormData.Bunker_Rob_Instructions = rawData.Bunker_Rob_Instructions;
            if (rawData.Robs != null && rawData.Robs.Count > 0)
            {
              var ForOb = new FOROB();
              var RowObj = new Robs
              {
                AsOfDate = rawData.Forob_AsOfDate
              };
              var RobList = new List<Rob>();
              foreach (var ForobObj in rawData.Robs)
              {
                var robObj = new Rob
                {
                  FuelType = ForobObj.FuelType,
                  Remaining = ForobObj.Remaining,
                  AuxEngineConsumption = ForobObj.AuxEngineConsumption,
                  BoilerEngineConsumption = ForobObj.BoilerEngineConsumption,
                  Units = ForobObj.Units,
                  Received = ForobObj.Received,
                  Consumption = ForobObj.Consumption
                };
                var Allocation = new List<Allocation>();
                foreach (var allocationObj in ForobObj.Allocation)
                {
                  var AllocationModel = new Allocation
                  {
                    Name = allocationObj.Name,
                    text = allocationObj.Text

                  };
                  Allocation.Add(AllocationModel);
                }
                robObj.Allocation = Allocation;
                RobList.Add(robObj);
              }
              RowObj.Rob = RobList;
              ForOb.Robs = RowObj;
              FormData.FOROB = ForOb;
            }
            FormData.Reason_for_Other_Consumption1 = rawData.Reason_for_Other_Consumption1;
            FormData.Fresh_Water_Produced_since_last_report_Mts = rawData.Fresh_Water_Produced_since_last_report_Mts;
            FormData.FreshWaterROB = rawData.FreshWaterROB;
            FormData.Fresh_Water_for_Tank_Cleaning = rawData.Fresh_Water_for_Tank_Cleaning;

            if (rawData.Auxilliary_Engine_Hours_Dtos != null && rawData.Auxilliary_Engine_Hours_Dtos.Count > 0)
            {
              var AuxilliaryList = new AuxilliaryEngineHours();
              var AuxList = new List<AuxilliaryEngineHoursRow>();
              foreach (var AuxilliaryObj in rawData.Auxilliary_Engine_Hours_Dtos)
              {
                var RowObj = new AuxilliaryEngineHoursRow
                {
                  Aux_Engine = AuxilliaryObj.Aux_Engine,
                  Hours = AuxilliaryObj.Hours
                };
                AuxList.Add(RowObj);
              }
              AuxilliaryList.Auxilliary_Engine_HoursRow = AuxList;
              FormData.Auxilliary_Engine_Hours = AuxilliaryList;
            }
            if (rawData.BunkerStemsAndSurveysDtos != null && rawData.BunkerStemsAndSurveysDtos.Count > 0)
            {
              var BunkerStemsAndSurveysList = new BunkerStemsAndSurveys();
              var SteamSurveyList = new List<BunkerStemsAndSurveysRow>();
              foreach (var SteamSurveyObj in rawData.BunkerStemsAndSurveysDtos)
              {
                var RowObj = new BunkerStemsAndSurveysRow
                {
                  Number = SteamSurveyObj.Number,
                  Grade = SteamSurveyObj.Grade,
                  Stem_or_Survey = SteamSurveyObj.StemOrSurvey,
                  BDN_Figure = SteamSurveyObj.BdnFigure,
                  Ships_Received_Figure = SteamSurveyObj.ShipsReceivedFigure,
                  Survey_ROB_Differential = SteamSurveyObj.SurveyRobDifferential
                };
                SteamSurveyList.Add(RowObj);
              }
              BunkerStemsAndSurveysList.Bunker_Stems_and_SurveysRow = SteamSurveyList;

              FormData.Bunker_Stems_and_Surveys = BunkerStemsAndSurveysList;
            }
            if (rawData.BunkerTanks_Dtos != null && rawData.BunkerTanks_Dtos.Count > 0)
            {
              var BunkerList = new BunkerTanks();
              var BunkerTanksRowList = new List<BunkerTanksRow>();
              foreach (var BunkerObj in rawData.BunkerTanks_Dtos)
              {
                var RowObj = new BunkerTanksRow
                {
                  BunkerTankNo = BunkerObj.BunkerTankNo,
                  BunkerTankDescription = BunkerObj.BunkerTankDescription,
                  BunkerTankFuelGrade = BunkerObj.BunkerTankFuelGrade,
                  BunkerTankCapacity = BunkerObj.BunkerTankCapacity,
                  BunkerTankObservedVolume = BunkerObj.BunkerTankObservedVolume,
                  BunkerTankUnit = BunkerObj.BunkerTankUnit,
                  BunkerTankROB = BunkerObj.BunkerTankROB,
                  BunkerTankFillPercent = BunkerObj.BunkerTankFillPercent,
                  BunkerTankSupplyDate = BunkerObj.BunkerTankSupplyDate
                };
                BunkerTanksRowList.Add(RowObj);

              }
              BunkerList.BunkerTanksRow = BunkerTanksRowList;

              FormData.BunkerTanks = BunkerList;
            }

            FormData.VesselCondition = rawData.VesselCondition;
            var JsonString = "";

            if (rawData.ReportType == ReportTypes.Position.ToString())
            {
              var positionReport = FormData.Adapt<PositionReportModel>();
              positionReport.Location = rawData.Location;
              positionReport.Event = rawData.Event;

              JsonString = JsonConvert.SerializeObject(positionReport);
            }
            else if (rawData.ReportType == ReportTypes.Departure.ToString())
            {
              var departureReport = FormData.Adapt<DepartureReportModel>();
              departureReport.SWBWFW = rawData.SWBWFW;
              departureReport.SWBWFW_Est = rawData.SWBWFW_Est;
              departureReport.Est_Fwd_Draft = rawData.Est_Fwd_Draft;
              departureReport.Est_Aft_Draft = rawData.Est_Aft_Draft;
              departureReport.Cargo_Grade = rawData.Cargo_Grade;
              departureReport.Total_Cargo_on_Board = rawData.Total_Cargo_on_Board;
              departureReport.FreshWaterMade = rawData.FreshWaterMade;
              departureReport.SlopsROB = rawData.SlopsROB;
              departureReport.Grade_of_Slops = rawData.Grade_of_Slops;
              departureReport.Slop_Stowage = rawData.Slop_Stowage;
              departureReport.Slops__Annex_1_or_Annex_2 = rawData.Slops__Annex_1_or_Annex_2;
              departureReport.Inspections_In_Port_1 = rawData.Inspections_In_Port_1;
              departureReport.Inspections_In_Port_2 = rawData.Inspections_In_Port_2;
              departureReport.Inspections_In_Port_3 = rawData.Inspections_In_Port_3;
              departureReport.Inspect_Co_1 = rawData.Inspect_Co_1;
              departureReport.Inspect_Co_2 = rawData.Inspect_Co_2;
              departureReport.Inspect_Co_3 = rawData.Inspect_Co_3;
              departureReport.Date_1 = rawData.Date_1;
              departureReport.Date_2 = rawData.Date_2;
              departureReport.Date_3 = rawData.Date_3;
              departureReport.Number_of_Tugs_In = rawData.Number_of_Tugs_In;
              departureReport.Tug_In_Start_Time = rawData.Tug_In_Start_Time;
              departureReport.Number_of_Tugs_Out = rawData.Number_of_Tugs_Out;
              departureReport.Tugs_Out_Start_Time = rawData.Tugs_Out_Start_Time;
              departureReport.Number_of_Shifting_Tugs = rawData.Number_of_Shifting_Tugs;
              departureReport.Shifting_Tugs_Start_Time = rawData.Shifting_Tugs_Start_Time;
              departureReport.Berth_Shifted_To__In = rawData.Berth_Shifted_To__In;
              departureReport.Tugs_in_End_Time = rawData.Tugs_in_End_Time;
              departureReport.Berth_Shifted_From = rawData.Berth_Shifted_From;
              departureReport.Berth_Shifted_To = rawData.Berth_Shifted_To;
              departureReport.Tugs_Out_End_Time = rawData.Tugs_Out_End_Time;
              departureReport.Shifting_Tugs_End_Time = rawData.Shifting_Tugs_End_Time;
              departureReport.PEId = rawData.PEId;
              JsonString = JsonConvert.SerializeObject(departureReport);
            }
            else if (rawData.ReportType == ReportTypes.Arrival.ToString())
            {
              var arrivalReport = FormData.Adapt<ArrivalReportModel>();
              arrivalReport.Arrival_Position = rawData.Arrival_Position;
              arrivalReport.NOR_Tendered_Time = rawData.NOR_Tendered_Time;
              arrivalReport.Time_Drop_of_Anchor = rawData.Time_Drop_of_Anchor;
              arrivalReport.Free_Pratique_Granted = rawData.Free_Pratique_Granted;
              arrivalReport.ETB = rawData.Etb;
              arrivalReport.LOP_issued = rawData.LOP_issued;
              arrivalReport.SlopsROB = rawData.SlopsROB;
              arrivalReport.Grade_of_Slops = rawData.Grade_of_Slops;
              arrivalReport.Slop_Stowage = rawData.Slop_Stowage;
              arrivalReport.Slops__Annex_1_or_Annex_2 = rawData.Slops__Annex_1_or_Annex_2;
              arrivalReport.PSId = rawData.PSId;

              JsonString = JsonConvert.SerializeObject(arrivalReport);
            }
            //convret json to xml
            XmlDocument? doc = JsonConvert.DeserializeXmlNode(JsonString, "Form");
            XMLList.Add(doc.OuterXml);
            //send data to veslink api

            RestClient restClient = new("https://api.veslink.com/v1/forms/submit?apiToken=6d457574560f9eb91e0628a95293b530b3feeb7204a3daeac3e1048bdda30ed5");

            RestRequest restRequest = new("https://api.veslink.com/v1/forms/submit?apiToken=6d457574560f9eb91e0628a95293b530b3feeb7204a3daeac3e1048bdda30ed5", Method.Post);
            restRequest.AddXmlBody(doc.OuterXml);

            RestResponse restResponse = restClient.ExecutePost(restRequest);

            if (restResponse == null || !restResponse.IsSuccessful || restResponse.Content == null)
            {
              _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
              {
                StartedDateTime = startedDateTime,
                EndedDateTime = DateTime.UtcNow,
                ResourceUri = restResponse?.ResponseUri,
                RequestJSONBody = JsonConvert.SerializeObject(restRequest),
                ResponseJSONBody = JsonConvert.SerializeObject(restResponse),
                RecordsToBeProcessed = resData.Item1.Count,
                RecordsActuallyProcessed = rawDataIndex,
                Content = restResponse?.Content,
                StatusCode = restResponse?.StatusCode != null ? Convert.ToString((int)restResponse?.StatusCode) : string.Empty,
                StatusDescription = restResponse?.StatusDescription,
                Source = restResponse?.ErrorException?.Source,
                Message = restResponse?.ErrorException?.Message,
                InnerException = restResponse?.ErrorException?.InnerException?.ToString(),
                StackTrace = restResponse?.ErrorException?.StackTrace
              });

              continue;
            }
            else if (restResponse != null && restResponse.Content != null && restResponse.IsSuccessStatusCode)
            {
              rawDataIndex++;

              XmlDocument? doc1 = new();
              doc1 = JsonConvert.DeserializeXmlNode(restResponse.Content, "FormSubmissionResponse");
              var FormResponse = doc1?.GetElementsByTagName("FormSubmissionResponse");
              var GetResponse = Convert.ToBoolean(FormResponse?[0]?.ChildNodes?[1]?.InnerXml);

              if (GetResponse)
              {
                _serviceManager.VesselRawDataService.UpdateData(VesselObj);
              }

              var jsonConvert = JsonConvert.DeserializeObject<VeslinkResponseDto>(restResponse.Content);

              if (!Convert.ToBoolean(jsonConvert?.success) || jsonConvert?.formErrors?.Length > 0)
              {
                _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                {
                  StartedDateTime = startedDateTime,
                  EndedDateTime = DateTime.UtcNow,
                  ResourceUri = restResponse?.ResponseUri,
                  RecordsToBeProcessed = resData.Item1.Count,
                  RecordsActuallyProcessed = rawDataIndex,
                  Content = JsonConvert.SerializeObject(jsonConvert?.formErrors),
                  StatusCode = restResponse?.StatusCode != null ? Convert.ToString((int)restResponse?.StatusCode) : string.Empty,
                  StatusDescription = restResponse?.StatusDescription,
                  Source = restResponse?.ErrorException?.Source,
                  Message = restResponse?.ErrorException?.Message,
                  InnerException = restResponse?.ErrorException?.InnerException?.ToString(),
                  StackTrace = restResponse?.ErrorException?.StackTrace
                });
              }
            }
          }

          Log.Information($"Actually {rawDataIndex} records have been processed successfully.");
        }
        //Console.WriteLine("testing..."+DateTime.Now);
        return Task.CompletedTask;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
      return base.StopAsync(cancellationToken);
    }
  }
}
