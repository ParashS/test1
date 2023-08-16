using Contracts;
using DataTransferObjects.ReportForm;
using DataTransferObjects.VesselsRawData;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using Serilog;
using ServiceContracts.EntityContracts;
using Services.Helpers;
using System.Data;
using System.Text;


namespace Services.EntityServices
{
    public class VesselRawDataService : IVesselRawDataService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ShipWatchDataContext _dataContext;

        public VesselRawDataService(IRepositoryWrapper repositoryWrapper, ShipWatchDataContext dataContext)
        {
            _repositoryWrapper = repositoryWrapper;
            _dataContext = dataContext;
        }

        public async Task<long> CreateAsync(VesselsRawDataDto request)
        {
            var (ParentVesselDataRawId, latestVersionNumber) = GetMaxVersionNumberByReport(new GetVersionNumberByReportRequestDto()
            {
                FormIdentifier = request.FormIdentifier,
                ReportTime = request.ReportTime,
                ImoNumber = request.ImoNumber,
                ReportType = request.ReportType
            });

            TbVesselRawDatum tbVesselRawDatum = new()
            {
                ChiefEngineerName = request.ChiefEngineerName,
                MasterName = request.MasterName,
                SourceFeed = request.SourceFeed,
                AttachmentId = request.AttachmentId,
                ParentVesselRawDataId = ParentVesselDataRawId
            };
            if (!string.IsNullOrEmpty(request.FormIdentifier))
            {
                tbVesselRawDatum.ReportType = StaticData.ReportTypeNames[request.FormIdentifier ?? ""];
            }
            else
            {
                tbVesselRawDatum.ReportType = request.ReportType;
            }
            tbVesselRawDatum.VersionNumber = latestVersionNumber + 1; // Version Number
            tbVesselRawDatum.FormIdentifier = request.FormIdentifier;
            tbVesselRawDatum.CompanyCode = request.CompanyCode;
            tbVesselRawDatum.CompanyName = request.CompanyName;
            tbVesselRawDatum.VesselCode = request.VesselCode;
            tbVesselRawDatum.SubmittedDate = request.SubmittedDate;
            tbVesselRawDatum.Status = request.Status;
            tbVesselRawDatum.ApprovedDate = request.ApprovedDate;
            tbVesselRawDatum.LatestResubmissionDate = request.LatestResubmissionDate;
            tbVesselRawDatum.FormGuid = request.FormGUID;
            tbVesselRawDatum.ImoNumber = request.ImoNumber;
            tbVesselRawDatum.UnCode = request.UnCode;

            tbVesselRawDatum.VesselName = request.VesselName;
            tbVesselRawDatum.VoyageNo = request.VoyageNo;
            tbVesselRawDatum.VesselCondition = request.VesselCondition;
            tbVesselRawDatum.Fwddraft = request.FWDDraft;
            tbVesselRawDatum.Aftdraft = request.AFTDraft;
            tbVesselRawDatum.ArrivalPosition = request.Arrival_Position;
            tbVesselRawDatum.Port = request.Port;
            tbVesselRawDatum.Latitude = request.Latitude;
            tbVesselRawDatum.Longitude = request.Longitude;
            tbVesselRawDatum.ReportTime = request.ReportTime;
            tbVesselRawDatum.ReportTimeString = request.ReportTimeString;
            tbVesselRawDatum.TimeDropOfAnchor = request.Time_Drop_of_Anchor;
            tbVesselRawDatum.SteamingHrs = request.SteamingHrs;
            tbVesselRawDatum.MainEngineHrs = request.MainEngineHrs;
            tbVesselRawDatum.ObservedDistance = request.ObservedDistance;
            tbVesselRawDatum.EngineDistance = request.EngineDistance;
            tbVesselRawDatum.LoggedDistance = request.Logged_Distance;
            tbVesselRawDatum.ObsSpeed = request.ObsSpeed;
            tbVesselRawDatum.Cpspeed = request.CPSpeed;
            tbVesselRawDatum.LoggedSpeed = request.Logged_Speed;
            tbVesselRawDatum.Rpm = request.RPM;
            tbVesselRawDatum.MeoutputPct = request.MEOutputPct;
            tbVesselRawDatum.Slip = request.Slip;
            tbVesselRawDatum.WindForce = request.WindForce;
            tbVesselRawDatum.WindDirection = request.WindDirection;

            tbVesselRawDatum.ForobAsOfDate = request.Forob_AsOfDate;
            tbVesselRawDatum.FreshWaterProducedSinceLastReportMts = request.Fresh_Water_Produced_since_last_report_Mts;
            tbVesselRawDatum.FreshWaterRob = request.FreshWaterROB;
            tbVesselRawDatum.FreshWaterForTankCleaning = request.Fresh_Water_for_Tank_Cleaning;
            tbVesselRawDatum.Etd = request.ETD;
            tbVesselRawDatum.NorTenderedTime = request.NOR_Tendered_Time;
            tbVesselRawDatum.LopIssued = request.LOP_issued;
            tbVesselRawDatum.FreePratiqueGranted = request.Free_Pratique_Granted;
            tbVesselRawDatum.Etb = request.Etb;
            tbVesselRawDatum.Stoppages = request.Stoppages;
            tbVesselRawDatum.InstructedRpm = request.Instructed_RPM;
            tbVesselRawDatum.Remarks = request.Remarks;
            tbVesselRawDatum.DistanceVesselExplanation = request.Distance__Vessel_Explanation;
            tbVesselRawDatum.AuxiliaryEngineInstructions = request.Auxiliary_Engine_Instructions;

            tbVesselRawDatum.ReasonToOmitReportFromPerformance = request.Reason_to_omit_report_from_Performance;
            tbVesselRawDatum.SlopsRob = request.SlopsROB;
            tbVesselRawDatum.SlopsAnnex1OrAnnex2 = request.Slops__Annex_1_or_Annex_2;
            tbVesselRawDatum.GradeOfSlops = request.Grade_of_Slops;
            tbVesselRawDatum.SlopStowage = request.Slop_Stowage;
            tbVesselRawDatum.BunkerTankInstructions = request.Bunker_Tank_Instructions;
            tbVesselRawDatum.BunkerRobInstructions = request.Bunker_Rob_Instructions;
            tbVesselRawDatum.ReasonForOtherConsumption1 = request.Reason_for_Other_Consumption1;
            tbVesselRawDatum.Psid = request.PSId;
            tbVesselRawDatum.Location = request.Location;
            tbVesselRawDatum.ReportLocation = request.ReportLocation;
            tbVesselRawDatum.Event = request.Event;

            tbVesselRawDatum.DistanceToGo = request.DistanceToGo;
            tbVesselRawDatum.Swbwfw = request.SWBWFW;
            tbVesselRawDatum.EstFwdDraft = request.Est_Fwd_Draft;
            tbVesselRawDatum.EstAftDraft = request.Est_Aft_Draft;
            tbVesselRawDatum.SwbwfwEst = request.SWBWFW_Est;
            tbVesselRawDatum.TotalCargoOnBoard = request.Total_Cargo_on_Board;
            tbVesselRawDatum.CargoGrade = request.Cargo_Grade;
            tbVesselRawDatum.FreshWaterMade = request.FreshWaterMade;
            tbVesselRawDatum.BunkerStemAndSurveyDescription = request.Bunker_Stem_and_Survey_Description;
            tbVesselRawDatum.InspectionsInPort1 = request.Inspections_In_Port_1;
            tbVesselRawDatum.InspectionsInPort2 = request.Inspections_In_Port_2;
            tbVesselRawDatum.InspectionsInPort3 = request.Inspections_In_Port_3;
            tbVesselRawDatum.InspectCo1 = request.Inspect_Co_1;
            tbVesselRawDatum.InspectCo2 = request.Inspect_Co_2;
            tbVesselRawDatum.InspectCo3 = request.Inspect_Co_3;
            tbVesselRawDatum.Date1 = request.Date_1;
            tbVesselRawDatum.Date2 = request.Date_2;
            tbVesselRawDatum.Date3 = request.Date_3;
            tbVesselRawDatum.NumberOfTugsIn = request.Number_of_Tugs_In;
            tbVesselRawDatum.TugInStartTime = request.Tug_In_Start_Time;
            tbVesselRawDatum.NumberOfTugsOut = request.Number_of_Tugs_Out;
            tbVesselRawDatum.TugsOutStartTime = request.Tugs_Out_Start_Time;
            tbVesselRawDatum.NumberOfShiftingTugs = request.Number_of_Shifting_Tugs;
            tbVesselRawDatum.ShiftingTugsStartTime = request.Shifting_Tugs_Start_Time;
            tbVesselRawDatum.BerthShiftedToIn = request.Berth_Shifted_To__In;
            tbVesselRawDatum.TugsInEndTime = request.Tugs_in_End_Time;
            tbVesselRawDatum.BerthShiftedFrom = request.Berth_Shifted_From;
            tbVesselRawDatum.BerthShiftedTo = request.Berth_Shifted_To;
            tbVesselRawDatum.TugsOutEndTime = request.Tugs_Out_End_Time;
            tbVesselRawDatum.ShiftingTugsEndTime = request.Shifting_Tugs_End_Time;
            tbVesselRawDatum.Peid = request.PEId;
            tbVesselRawDatum.IsVrsProcessed = false;
            tbVesselRawDatum.IsNdrProcessed = false;
            tbVesselRawDatum.IsUnifiedMetricsProcessed = false;
            tbVesselRawDatum.CargoLoad = request.CargoLoad;
            tbVesselRawDatum.CargoDischarge = request.CargoDischarge;
            tbVesselRawDatum.StoppageRemarks = request.StoppageRemarks;
            tbVesselRawDatum.MEPowerkW = request.MEPowerkW;
            tbVesselRawDatum.ManouveringHours = request.ManouveringHours;
            tbVesselRawDatum.GmtOffset = request.GmtOffset;
            tbVesselRawDatum.DSS = request.DSS;
            tbVesselRawDatum.HoursSinceLastReport = request.HoursSinceLastReport;
            tbVesselRawDatum.RemarksforOtherCons = request.RemarksforOtherCons;
            if (!string.IsNullOrEmpty(request.Latitude) && !string.IsNullOrWhiteSpace(request.Latitude) &&
                !string.IsNullOrEmpty(request.Longitude) && !string.IsNullOrWhiteSpace(request.Longitude))
            {
                string DMSLatString = request.Latitude;
                string DMSLongString = request.Longitude;

                var LatMetaArray = DMSLatString.Replace("'", "").Replace("\"", "").Split(" ");
                var LongMetaArray = DMSLongString.Replace("'", "").Replace("\"", "").Split(" ");

                var latidudePoint = ConvertDegreeAngleToDouble(Convert.ToDouble(LatMetaArray[0]), Convert.ToDouble(LatMetaArray[1]), Convert.ToDouble(LatMetaArray[2]));
                var longitudePoint = ConvertDegreeAngleToDouble(Convert.ToDouble(LongMetaArray[0]), Convert.ToDouble(LongMetaArray[1]), Convert.ToDouble(LongMetaArray[2]));

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                tbVesselRawDatum.GeographyLocation = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate((double)longitudePoint, (double)latidudePoint));
            }
            tbVesselRawDatum.CreatedOn = DateTime.UtcNow;

            await _repositoryWrapper.VesselRawData.CreateAsync(tbVesselRawDatum);
            await _repositoryWrapper.SaveAync();

            if (request.Robs != null && request.Robs.Count > 0)
            {
                foreach (RobDto robItem in request.Robs)
                {
                    TbVesselForobRawDatum tbVesselForobRawData = new()
                    {
                        VesselDataRawId = tbVesselRawDatum.Id,
                        FuelType = robItem.FuelType,
                        Remaining = robItem.Remaining,
                        AuxEngineConsumption = robItem.AuxEngineConsumption,
                        BoilerEngineConsumption = robItem.BoilerEngineConsumption,
                        Units = robItem.Units,
                        Received = robItem.Received,
                        Consumption = robItem.Consumption
                    };

                    await _repositoryWrapper.VesselForobRawData.CreateAsync(tbVesselForobRawData);
                    await _repositoryWrapper.SaveAync();

                    if (robItem.Allocation != null && robItem.Allocation.Count > 0)
                    {
                        var tbVesselForobAllocationRawDatas = robItem.Allocation.Select(x => new TbVesselForobAllocationRawDatum()
                        {
                            VesselForobRawDataId = tbVesselForobRawData.Id,
                            Name = x.Name,
                            Text = x.Text
                        }).ToList();

                        await _repositoryWrapper.VesselForobAllocationRawData.CreateRangeAsync(tbVesselForobAllocationRawDatas);
                        await _repositoryWrapper.SaveAync();
                    }
                }
            }

            if (request.Upcomings != null && request.Upcomings.Count > 0)
            {
                var tbVesselUpcomingPortRawDatas = request.Upcomings.Select(x => new TbVesselUpcomingPortRawDatum()
                {
                    VesselDataRawId = tbVesselRawDatum.Id,
                    PortName = x.PortName,
                    DistToGo = x.DistToGo,
                    ProjSpeed = x.ProjSpeed,
                    Eta = x.Eta,
                    Via = x.Via,
                    UnCode = x.UnCode
                }).ToList();

                await _repositoryWrapper.VesselUpcomingPortRawData.CreateRangeAsync(tbVesselUpcomingPortRawDatas);
                await _repositoryWrapper.SaveAync();
            }

            if (request.BunkerTanks_Dtos != null && request.BunkerTanks_Dtos.Count > 0)
            {
                List<TbVesselBunkerTankRawDatum> tbVesselBunkerTankRawDatas = request.BunkerTanks_Dtos.Select(bunkerItem =>
                new TbVesselBunkerTankRawDatum()
                {
                    VesselDataRawId = tbVesselRawDatum.Id,
                    BunkerTankNo = bunkerItem.BunkerTankNo,
                    BunkerTankDescription = bunkerItem.BunkerTankDescription,
                    BunkerTankFuelGrade = bunkerItem.BunkerTankFuelGrade,
                    BunkerTankCapacity = bunkerItem.BunkerTankCapacity,
                    BunkerTankObservedVolume = bunkerItem.BunkerTankObservedVolume,
                    BunkerTankUnit = bunkerItem.BunkerTankUnit,
                    BunkerTankRob = bunkerItem.BunkerTankROB,
                    BunkerTankFillPercent = bunkerItem.BunkerTankFillPercent,
                    BunkerTankSupplyDate = bunkerItem.BunkerTankSupplyDate
                }).ToList();

                await _repositoryWrapper.VesselBunkerTankRawData.CreateRangeAsync(tbVesselBunkerTankRawDatas);
                await _repositoryWrapper.SaveAync();
            }

            if (request.Auxilliary_Engine_Hours_Dtos != null && request.Auxilliary_Engine_Hours_Dtos.Count > 0)
            {
                List<TbVesselsAuxilliaryRawDatum> tbVesselsAuxilliaryRawData = request.Auxilliary_Engine_Hours_Dtos.Select(auxItem =>
                new TbVesselsAuxilliaryRawDatum()
                {
                    VesselDataRawId = tbVesselRawDatum.Id,
                    AuxEngine = auxItem.Aux_Engine,
                    Hours = auxItem.Hours
                }).ToList();

                await _repositoryWrapper.VesselsAuxilliaryRawData.CreateRangeAsync(tbVesselsAuxilliaryRawData);
                await _repositoryWrapper.SaveAync();
            }

            if (request.BunkerStemsAndSurveysDtos != null && request.BunkerStemsAndSurveysDtos.Count > 0)
            {
                List<TbVesselBunkerStemsAndSurveysRawDatum> tbVesselBunkerStemsAndSurveysRawDatas = request.BunkerStemsAndSurveysDtos.Select(bunkerStemsAndSurveyItem =>
                new TbVesselBunkerStemsAndSurveysRawDatum()
                {
                    VesselRawDataId = tbVesselRawDatum.Id,
                    BdnFigure = bunkerStemsAndSurveyItem.BdnFigure,
                    Grade = bunkerStemsAndSurveyItem.Grade,
                    Number = bunkerStemsAndSurveyItem.Number,
                    ShipsReceivedFigure = bunkerStemsAndSurveyItem.ShipsReceivedFigure,
                    StemOrSurvey = bunkerStemsAndSurveyItem.StemOrSurvey,
                    SurveyRobDifferential = bunkerStemsAndSurveyItem.SurveyRobDifferential
                }).ToList();

                await _repositoryWrapper.VesselBunkerStemsAndSurveysRawData.CreateRangeAsync(tbVesselBunkerStemsAndSurveysRawDatas);
                await _repositoryWrapper.SaveAync();
            }

            if (request.CargoHandling != null && request.CargoHandling.Count > 0)
            {
                List<TbCargoHandlingDatum> tbCargoHandlingData = new();
                foreach (var data in request.CargoHandling)
                {
                    var tbCargoHandlingObj = new TbCargoHandlingDatum
                    {
                        VesselDataRawId = tbVesselRawDatum.Id,
                        CargoTypeId = data.CargoTypeID,
                        Function = data.Function,
                        Berth = data.Berth,
                        BLDate = data.BLDate,
                        BLCode = data.BLCode,
                        BLGross = data.BLGross,
                        CargoName = data.CargoName,
                        ShipGross = data.ShipGross,
                        LoadTemp = data.LoadTemp,
                        ApiGravity = data.APIGravity,
                        UnitCode = data.UnitCode,
                        Charterer = data.Charterer,
                        Consignee = data.Consignee,
                        Receiver = data.Receiver,
                        Shipper = data.Shipper,
                        Destination = data.Destination,
                        LetterOfProtest = data.LetterOfProtest
                    };

                    await _repositoryWrapper.CargoHandlingData.CreateAsync(tbCargoHandlingObj);
                    await _repositoryWrapper.SaveAync();
                    if (data.Stowage != null && data.Stowage.Count > 0)
                    {
                        var tbStowageList = data.Stowage.Select(x => new TbStowagesDatum()
                        {
                            CargoId = tbCargoHandlingObj.CargoId,
                            TankName = x.TankName,
                            Quantity = x.Quantity,
                            Unit = x.Unit
                        }).ToList();
                        await _repositoryWrapper.StowageData.CreateRangeAsync(tbStowageList);
                        await _repositoryWrapper.SaveAync();
                    }
                }
            }

            if (request.PortActivities != null && request.PortActivities.Count > 0)
            {
                List<TbPortActivities> tbPortActivities = request.PortActivities.Select(x => new TbPortActivities()
                {
                    VesselDataRawID = tbVesselRawDatum.Id,
                    ActivityName = x.ActivityName,
                    Time = x.Time,
                    CargoName = x.CargoName,
                    Charterer = x.Charterer,
                    Remark = x.Remarks,
                    Berth = x.Berth
                }).ToList();
                await _repositoryWrapper.PortActivities.CreateRangeAsync(tbPortActivities);
                await _repositoryWrapper.SaveAync();
            }

            return tbVesselRawDatum.Id;
        }
        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600

            return degrees + (minutes / 60) + (seconds / 3600);
        }

        public (long?, int) GetMaxVersionNumberByReport(GetVersionNumberByReportRequestDto request)
        {
            try
            {
                int MaxVersionNumber = 0;
                long? ParentVesselRawDataId = 0;

                if (_repositoryWrapper.VesselRawData.GetByCondition(x =>
                        (x.FormIdentifier == request.FormIdentifier || x.ReportType == request.ReportType) &&
                        x.ReportTime != null && x.ReportTime.Value == request.ReportTime &&
                        x.ImoNumber == request.ImoNumber
                    ).Count() == 0)
                {
                    return (null, 0);
                }
                if (!string.IsNullOrEmpty(request.FormIdentifier))
                {
                    if (StaticData.ReportTypeDictionary[request.FormIdentifier ?? ""] == ReportTypes.Position)
                    {
                        var filteredVessels = _repositoryWrapper.VesselRawData.GetByCondition(x => x.FormIdentifier == request.FormIdentifier &&
                                                                                                x.ReportTime != null && x.ReportTime.Value == request.ReportTime &&
                                                                                                x.ImoNumber == request.ImoNumber
                                                                                            ).ToList();

                        if (filteredVessels == null || filteredVessels.Count == 0)
                            return (null, 0);

                        MaxVersionNumber = filteredVessels.Max(x => x.VersionNumber);

                        ParentVesselRawDataId = filteredVessels.Count == 1 ? filteredVessels.First().Id : filteredVessels.Where(x => x.VersionNumber == MaxVersionNumber).First().Id;
                    }
                    else  // Report Type = "Arrival" OR Report Type = "Departure" 
                    {
                        var filteredVessels = _repositoryWrapper.VesselRawData.GetByCondition(x =>
                            x.FormIdentifier == request.FormIdentifier &&
                            x.ReportTime.HasValue && x.ReportTime.Value == request.ReportTime &&
                            x.ImoNumber == request.ImoNumber &&
                            x.LatestResubmissionDate != request.LatestResubmissionDate
                        ).ToList();

                        if (filteredVessels == null || filteredVessels.Count == 0)
                            return (null, 0);

                        MaxVersionNumber = filteredVessels.Max(x => x.VersionNumber);

                        ParentVesselRawDataId = filteredVessels.Count == 1 ? filteredVessels.First().Id : filteredVessels.Where(x => x.VersionNumber == MaxVersionNumber).First().Id;
                    }
                }
                else
                {
                    if (request.ReportType == ReportTypes.Position.ToString())
                    {
                        var filteredVessels = _repositoryWrapper.VesselRawData.GetByCondition(x => x.ReportType == request.ReportType &&
                                                                                                x.ReportTime != null && x.ReportTime.Value == request.ReportTime &&
                                                                                                x.ImoNumber == request.ImoNumber
                                                                                            ).ToList();

                        if (filteredVessels == null || filteredVessels.Count == 0)
                            return (null, 0);

                        MaxVersionNumber = filteredVessels.Max(x => x.VersionNumber);

                        ParentVesselRawDataId = filteredVessels.Count == 1 ? filteredVessels.First().Id : filteredVessels.Where(x => x.VersionNumber == MaxVersionNumber).First().Id;
                    }
                    else  // Report Type = "Arrival" OR Report Type = "Departure" 
                    {
                        var filteredVessels = _repositoryWrapper.VesselRawData.GetByCondition(x =>
                            x.ReportType == request.ReportType &&
                            x.ReportTime.HasValue && x.ReportTime.Value == request.ReportTime &&
                            x.ImoNumber == request.ImoNumber
                        ).ToList();

                        if (filteredVessels == null || filteredVessels.Count == 0)
                            return (null, 0);

                        MaxVersionNumber = filteredVessels.Max(x => x.VersionNumber);

                        ParentVesselRawDataId = filteredVessels.Count == 1 ? filteredVessels.First().Id : filteredVessels.Where(x => x.VersionNumber == MaxVersionNumber).First().Id;
                    }
                }
                return (ParentVesselRawDataId, MaxVersionNumber);
            }
            catch (Exception)
            {
                return (null, 0);
            }
        }

        public async Task<List<string?>> GetAllExistedAttachmentIdAsync()
        {
            return await _repositoryWrapper.VesselRawData.GetAll().Where(x => x.AttachmentId != null).Select(x => x.AttachmentId).ToListAsync();
        }

        public (List<VesselsRawDataDto>, List<TbVesselRawDatum>) GetData()
        {
            var VesselRawDataList = new List<VesselsRawDataDto>();
            //  var RawData = _repositoryWrapper.VesselRawData.GetByCondition(x => x.CompanyName == "test" && x.ImoNumber == "9833682" && x.CreatedOn >= DateTime.Now.AddHours(-1)).ToList();
            var RawData = _repositoryWrapper.VesselRawData.GetByCondition(x => x.CompanyName == "test" && x.ImoNumber == "9833682" && (x.IsSentToVesLink == null || x.IsSentToVesLink == false)).ToList();
            // var RawData = _repositoryWrapper.VesselRawData.GetByCondition(x => x.CompanyName == "test" && x.ImoNumber == "9833682" ).ToList();
            foreach (var data in RawData)
            {
                var VesselRawData = new VesselsRawDataDto
                {
                    VesselRawDataId = data.Id,
                    FormIdentifier = data.FormIdentifier,
                    CompanyCode = data.CompanyCode,
                    CompanyName = data.CompanyName,
                    VesselCode = data.VesselCode,
                    SubmittedDate = data.SubmittedDate,
                    Status = data.Status,
                    ApprovedDate = data.ApprovedDate,
                    LatestResubmissionDate = data.LatestResubmissionDate,
                    FormGUID = data.FormGuid,
                    ImoNumber = data.ImoNumber,
                    VesselName = data.VesselName,
                    ReportTime = data.ReportTime,
                    ReportTimeString = data.ReportTimeString,
                    VoyageNo = data.VoyageNo,
                    FWDDraft = data.Fwddraft,
                    AFTDraft = data.Aftdraft,
                    Location = data.Location,
                    Event = data.Event,
                    Port = data.Port,
                    ETD = data.Etd,
                    Latitude = data.Latitude,
                    Longitude = data.Longitude,
                    ReportType = data.ReportType,

                    Upcomings = _repositoryWrapper.VesselUpcomingPortRawData.GetByCondition(x => x.VesselDataRawId == data.Id).Select(x => new UpcomingDto()
                    {
                        PortName = x.PortName,
                        DistToGo = x.DistToGo,
                        ProjSpeed = x.ProjSpeed,
                        Eta = x.Eta,
                        Via = x.Via,
                        UnCode = x.UnCode
                    }).ToList(),
                    SteamingHrs = data.SteamingHrs,
                    MainEngineHrs = data.MainEngineHrs,
                    Stoppages = data.Stoppages,
                    Instructed_RPM = data.InstructedRpm,
                    ObservedDistance = data.ObservedDistance,
                    EngineDistance = data.EngineDistance,
                    Logged_Distance = data.LoggedDistance,
                    Remarks = data.Remarks,
                    ObsSpeed = data.ObsSpeed,
                    CPSpeed = data.Cpspeed,
                    Logged_Speed = data.LoggedSpeed,
                    Distance__Vessel_Explanation = data.DistanceVesselExplanation,
                    RPM = data.Rpm,
                    MEOutputPct = data.MeoutputPct,
                    Slip = data.Slip,
                    Auxiliary_Engine_Instructions = data.AuxiliaryEngineInstructions,
                    WindDirection = data.WindDirection,
                    WindForce = data.WindForce,
                    Reason_to_omit_report_from_Performance = data.ReasonToOmitReportFromPerformance,
                    Bunker_Stem_and_Survey_Description = data.BunkerStemAndSurveyDescription,
                    Bunker_Tank_Instructions = data.BunkerTankInstructions,
                    Bunker_Rob_Instructions = data.BunkerRobInstructions,
                    Forob_AsOfDate = data.ForobAsOfDate,
                    SlopsROB = data.SlopsRob,
                    Slops__Annex_1_or_Annex_2 = data.SlopsAnnex1OrAnnex2,
                    Grade_of_Slops = data.GradeOfSlops,
                    Slop_Stowage = data.SlopStowage,
                    //var query=from 
                    Robs = _repositoryWrapper.VesselForobRawData.GetByCondition(x => x.VesselDataRawId == data.Id).AsEnumerable().Select(x => new RobDto()
                    {
                        VesselFobId = x.Id,
                        VesselDataRawId = x.VesselDataRawId,
                        FuelType = x.FuelType,
                        Remaining = x.Remaining,
                        AuxEngineConsumption = x.AuxEngineConsumption,
                        BoilerEngineConsumption = x.BoilerEngineConsumption,
                        Units = x.Units,
                        Received = x.Received,
                        Consumption = x.Consumption,
                        Allocation = _repositoryWrapper.VesselForobAllocationRawData.GetByCondition(y => x.Id == y.VesselForobRawDataId).AsEnumerable().Select(z => new AllocationDto
                        {
                            Name = z.Name,
                            Text = z.Text
                        }).ToList()
                    }).ToList(),

                    SWBWFW = data.Swbwfw,
                    SWBWFW_Est = data.SwbwfwEst,
                    Est_Aft_Draft = data.EstAftDraft,
                    Est_Fwd_Draft = data.EstFwdDraft,
                    Cargo_Grade = data.CargoGrade,
                    Total_Cargo_on_Board = data.TotalCargoOnBoard,
                    Reason_for_Other_Consumption1 = data.ReasonForOtherConsumption1,
                    Fresh_Water_Produced_since_last_report_Mts = data.FreshWaterProducedSinceLastReportMts,
                    FreshWaterROB = data.FreshWaterRob,
                    Fresh_Water_for_Tank_Cleaning = data.FreshWaterForTankCleaning,
                    Auxilliary_Engine_Hours_Dtos = _repositoryWrapper.VesselsAuxilliaryRawData.GetByCondition(x => x.VesselDataRawId == data.Id)
                    .Select(x => new AuxilliaryEngineHoursDto()
                    {
                        Aux_Engine = x.AuxEngine,
                        Hours = x.Hours,
                    }).ToList(),
                    BunkerStemsAndSurveysDtos = _repositoryWrapper.VesselBunkerStemsAndSurveysRawData.GetByCondition(x => x.VesselRawDataId == data.Id)
                    .Select(x => new BunkerStemsAndSurveysDto()
                    {
                        Number = x.Number,
                        Grade = x.Grade,
                        StemOrSurvey = x.StemOrSurvey,
                        BdnFigure = x.BdnFigure,
                        ShipsReceivedFigure = x.ShipsReceivedFigure,
                        SurveyRobDifferential = x.SurveyRobDifferential
                    }).ToList(),
                    BunkerTanks_Dtos = _repositoryWrapper.VesselBunkerTankRawData.GetByCondition(x => x.VesselDataRawId == data.Id)
                    .Select(bunkerItem =>
                new BunkerTanksDto()
                {
                    BunkerTankNo = bunkerItem.BunkerTankNo,
                    BunkerTankDescription = bunkerItem.BunkerTankDescription,
                    BunkerTankFuelGrade = bunkerItem.BunkerTankFuelGrade,
                    BunkerTankCapacity = bunkerItem.BunkerTankCapacity,
                    BunkerTankObservedVolume = bunkerItem.BunkerTankObservedVolume,
                    BunkerTankUnit = bunkerItem.BunkerTankUnit,
                    BunkerTankROB = bunkerItem.BunkerTankRob,
                    BunkerTankFillPercent = bunkerItem.BunkerTankFillPercent,
                    BunkerTankSupplyDate = bunkerItem.BunkerTankSupplyDate
                }).ToList(),
                    VesselCondition = data.VesselCondition,
                    Inspections_In_Port_1 = data.InspectionsInPort1,
                    Inspections_In_Port_2 = data.InspectionsInPort2,
                    Inspections_In_Port_3 = data.InspectionsInPort3,
                    Inspect_Co_1 = data.InspectCo1,
                    Inspect_Co_2 = data.InspectCo2,
                    Inspect_Co_3 = data.InspectCo3,
                    Date_1 = data.Date1,
                    Date_2 = data.Date2,
                    Date_3 = data.Date3,
                    Number_of_Tugs_In = data.NumberOfTugsIn,
                    Tug_In_Start_Time = data.TugInStartTime,
                    Number_of_Tugs_Out = data.NumberOfTugsOut,
                    Tugs_Out_Start_Time = data.TugsOutStartTime,
                    Number_of_Shifting_Tugs = data.NumberOfShiftingTugs,
                    Shifting_Tugs_Start_Time = data.ShiftingTugsStartTime,
                    Berth_Shifted_To__In = data.BerthShiftedToIn,
                    Tugs_in_End_Time = data.TugsInEndTime,
                    Berth_Shifted_From = data.BerthShiftedFrom,
                    Berth_Shifted_To = data.BerthShiftedTo,
                    Tugs_Out_End_Time = data.TugsOutEndTime,
                    Shifting_Tugs_End_Time = data.ShiftingTugsEndTime,
                    PEId = data.Peid,
                    NOR_Tendered_Time = data.NorTenderedTime,
                    Time_Drop_of_Anchor = data.TimeDropOfAnchor,
                    Free_Pratique_Granted = data.FreePratiqueGranted,
                    Etb = data.Etb,
                    LOP_issued = data.LopIssued,
                    PSId = data.Psid
                };

                VesselRawDataList.Add(VesselRawData);
            }

            return (VesselRawDataList, RawData);
        }
        public void UpdateData(TbVesselRawDatum request)
        {
            request.IsSentToVesLink = true;
            _repositoryWrapper.VesselRawData.Update(request);
            _repositoryWrapper.Save();
        }

        public async Task<long> AddManualRawReportAsync(ReportFormDto reportForm)
        {
            var (ParentVesselDataRawId, latestVersionNumber) = GetMaxVersionNumberByReport(new GetVersionNumberByReportRequestDto()
            {
                ReportType = reportForm.ReportType,
                ReportTime = reportForm.ReportOverView?.LocalDateTime,
                ImoNumber = Convert.ToString(reportForm.BasicDetails?.VesselImo)
            });
            var context = new ShipWatchDataContext();
            using var transaction = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                TbVesselRawDatum tbVessel = new()
                {
                    VersionNumber = latestVersionNumber + 1,
                    Aftdraft = reportForm.Operational?.Aftdraft,
                    ImoNumber = Convert.ToString(reportForm.BasicDetails?.VesselImo),
                    VesselName = reportForm.BasicDetails?.VesselName,
                    ReportLocation = reportForm.BasicDetails?.Location,
                    Location = reportForm.BasicDetails?.Location != null && reportForm.BasicDetails?.Location == "At Sea" ? "N" : "S",
                    Event = reportForm.BasicDetails?.ReportEvent,
                    VoyageNo = Convert.ToInt32(reportForm.BasicDetails?.VoyageNo),
                    Latitude = reportForm.ReportOverView?.Latitude,
                    Longitude = reportForm.ReportOverView?.Longitude,
                    VesselCondition = reportForm.Operational?.VesselCondition,
                    Fwddraft = reportForm.Operational?.Fwddraft,
                    Remarks = reportForm.Operational?.Remarks,
                    SteamingHrs = reportForm.DistanceAndSpeed?.SteamingHours,
                    Stoppages = Convert.ToString(reportForm.DistanceAndSpeed?.Stoppages),
                    InstructedRpm = Convert.ToString(reportForm.DistanceAndSpeed?.InstructedRPM),
                    ObservedDistance = reportForm.DistanceAndSpeed?.ObservedDistance,
                    LoggedDistance = reportForm.DistanceAndSpeed?.LoggedDistance,
                    EngineDistance = reportForm.DistanceAndSpeed?.EngineDistance,
                    Rpm = reportForm.AvgEngineParameter?.RPM,
                    MainEngineHrs = reportForm.DistanceAndSpeed?.ManoeuvringHrs,
                    Slip = reportForm.AvgEngineParameter?.Slip,
                    WindDirection = reportForm.Weather?.WindDirection,
                    ReasonToOmitReportFromPerformance = reportForm.Weather?.ReasonToOmitReportFromPerformance,
                    FreshWaterRob = reportForm.FreshWater?.FreshWaterROB,
                    FreshWaterMade = reportForm.FreshWater?.FreshWaterProduced,
                    FreshWaterForTankCleaning = reportForm.FreshWater?.FreshWaterForTankCleaning,
                    MasterName = $"{reportForm.Officers?.MasterName} {reportForm.Officers?.MasterSurname}",
                    ChiefEngineerName = $"{reportForm.Officers?.ChiefEngineerName} {reportForm.Officers?.ChiefEngineerSurname}",
                    NorTenderedTime = reportForm.PortSof?.NorTenderedTime,
                    FreePratiqueGranted = reportForm.PortSof?.FreePartiqueGranted,
                    TimeDropOfAnchor = reportForm.PortSof?.TimeDropOfAnchor,
                    LopIssued = reportForm.PortSof?.LopIssued,
                    SlopsRob = Convert.ToString(reportForm.Slops?.SlopsRob),
                    GradeOfSlops = reportForm.Slops?.GradeOfSlops,
                    SlopsAnnex1OrAnnex2 = reportForm.Slops?.SlopsAnnex1OrAnnex2,
                    SlopStowage = reportForm.Slops?.SlopStowage,
                    Port = !string.IsNullOrWhiteSpace(reportForm.ReportOverView?.CurrentPort) ? reportForm.ReportOverView?.CurrentPort.ToUpper() : string.Empty,
                    NumberOfTugsIn = Convert.ToString(reportForm.Tugs?.NumberOfTugsIn),
                    TugInStartTime = reportForm.Tugs?.TugInStartTime,
                    TugsInEndTime = reportForm.Tugs?.TugsInEndTime,
                    NumberOfTugsOut = Convert.ToString(reportForm.Tugs?.NumberOfTugsOut),
                    TugsOutStartTime = reportForm.Tugs?.TugsOutStartTime,
                    TugsOutEndTime = reportForm.Tugs?.TugsOutEndTime,
                    NumberOfShiftingTugs = Convert.ToString(reportForm.Tugs?.NumberOfShiftingTugs),
                    BerthShiftedTo = reportForm.Tugs?.BerthShiftedTo,
                    ShiftingTugsStartTime = reportForm.Tugs?.ShiftingTugsStartTime,
                    ShiftingTugsEndTime = reportForm.Tugs?.ShiftingTugsEndTime,
                    WindForce = reportForm.Weather?.BeaufortForce,
                    ObsSpeed = reportForm.DistanceAndSpeed?.SpeedOverGround,
                    Cpspeed = reportForm.DistanceAndSpeed?.InstructedSpeed,
                    LoggedSpeed = reportForm.DistanceAndSpeed?.SpeedThroughWater,
                    MeoutputPct = reportForm.AvgEngineParameter?.MainEngineLoadOutput,
                    ReportType = reportForm.ReportType,
                    Etd = reportForm.ReportOverView?.PortEtd,
                    ReportTime = reportForm.ReportOverView?.LocalDateTime,
                    ReportTimeString = reportForm.ReportOverView?.LocalDateTimeString,
                    Swbwfw = reportForm.Operational?.SWBWFW,
                    CreatedOn = DateTime.UtcNow,
                    DSS = reportForm.Weather?.DSS,
                    BerthShiftedToIn = reportForm.Tugs?.FirstBerthIn,
                    Etb = reportForm.PortSof?.EstimatedBerthInTime,
                    InspectCo1 = reportForm.Inspection?.InspectCo1,
                    InspectCo2 = reportForm.Inspection?.InspectCo2,
                    InspectCo3 = reportForm.Inspection?.InspectCo3,
                    InspectionsInPort1 = reportForm.Inspection?.InspectionsInPort1,
                    InspectionsInPort2 = reportForm.Inspection?.InspectionsInPort2,
                    InspectionsInPort3 = reportForm.Inspection?.InspectionsInPort3,
                    Date1 = reportForm.Inspection?.Date1,
                    Date2 = reportForm.Inspection?.Date2,
                    Date3 = reportForm.Inspection?.Date3,
                    MEPowerkW = reportForm.AvgEngineParameter?.MainEnginePower,
                    EstAftDraft = reportForm.ReportOverView?.EstAftDraft,
                    EstFwdDraft = reportForm.ReportOverView?.EstFwdDraft,
                    SwbwfwEst = reportForm.ReportOverView?.SwbwfwEst,
                    ReasonForOtherConsumption1 = reportForm.ReasonForOtherConsumption,
                    TotalCargoOnBoard = reportForm.Cargo?.TotalCargoOnBoard,
                    CargoGrade = reportForm.Cargo?.CargoGrade,
                    CargoDischarge = Convert.ToString(reportForm.Cargo?.CargoDischarge),
                    CargoLoad = Convert.ToString(reportForm.Cargo?.CargoDischarge),
                    SourceFeed = "Manual",
                };
                await _repositoryWrapper.VesselRawData.CreateAsync(tbVessel);
                await _repositoryWrapper.SaveAync();

                if (reportForm.AuxEngine != null && reportForm.AuxEngine.Any())
                {
                    List<TbVesselsAuxilliaryRawDatum> AuxEngine = new();
                    for (int i = 0; i < reportForm.AuxEngine.Count; i++)
                    {
                        TbVesselsAuxilliaryRawDatum auxilliaryRawDatum = new()
                        {
                            AuxEngine = reportForm.AuxEngine[i].Name,
                            Hours = reportForm.AuxEngine[i].Hours,
                            VesselDataRawId = tbVessel.Id
                        };
                        AuxEngine.Add(auxilliaryRawDatum);
                    }
                    await _repositoryWrapper.VesselsAuxilliaryRawData.CreateRangeAsync(AuxEngine);
                }

                if (reportForm.UpcommingPorts != null && reportForm.UpcommingPorts.Any())
                {
                    List<TbVesselUpcomingPortRawDatum> portList = new();
                    for (int i = 0; i < reportForm.UpcommingPorts.Count; i++)
                    {
                        TbVesselUpcomingPortRawDatum upcomming = new()
                        {
                            PortName = reportForm.UpcommingPorts[i].PortName,
                            Eta = reportForm.UpcommingPorts[i].PortEta,
                            Via = reportForm.UpcommingPorts[i].Via,
                            DistToGo = reportForm.UpcommingPorts[i].DistToGo,
                            ProjSpeed = reportForm.UpcommingPorts[i].ProjSpeed,
                            VesselDataRawId = tbVessel.Id
                        };
                        portList.Add(upcomming);
                    }
                    await _repositoryWrapper.VesselUpcomingPortRawData.CreateRangeAsync(portList);
                }

                if (reportForm.FuelTypes != null && reportForm.FuelTypes.Count > 0 && reportForm.BunkerFuelUsages != null && reportForm.BunkerFuelUsages.Count > 0)
                {
                    List<TbVesselForobRawDatum> tbVesselForobRawDatumList = new List<TbVesselForobRawDatum>();

                    foreach (var item in reportForm.FuelTypes)
                    {
                        TbVesselForobRawDatum tbVesselForobRawDatum = new TbVesselForobRawDatum();
                        tbVesselForobRawDatum.VesselDataRawId = tbVessel.Id;
                        tbVesselForobRawDatum.FuelType = item.ToUpper();

                        tbVesselForobRawDatum.Remaining = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Name.Trim().Equals("ReportedRob", StringComparison.OrdinalIgnoreCase)).Select(x => Convert.ToDecimal(x.Value)).FirstOrDefault();

                        tbVesselForobRawDatum.AuxEngineConsumption = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Section.Equals("AUX", StringComparison.OrdinalIgnoreCase) && x.Value != null).Select(x => Convert.ToDecimal(x.Value)).ToList().Sum();

                        tbVesselForobRawDatum.BoilerEngineConsumption = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Section.Equals("Boiler", StringComparison.OrdinalIgnoreCase) && x.Value != null).Select(x => Convert.ToDecimal(x.Value)).ToList().Sum();

                        tbVesselForobRawDatum.Consumption = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Value != null).Select(x => Convert.ToDecimal(x.Value)).ToList().Sum();

                        tbVesselForobRawDatum.RobError = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Name.Trim().Equals("RobError", StringComparison.OrdinalIgnoreCase)).Select(x => Convert.ToDecimal(x.Value)).FirstOrDefault();

                        tbVesselForobRawDatum.ROBDifference = reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item, StringComparison.OrdinalIgnoreCase) && x.Name.Trim().Equals("ROBDifference", StringComparison.OrdinalIgnoreCase)).Select(x => Convert.ToDecimal(x.Value)).FirstOrDefault();

                        tbVesselForobRawDatumList.Add(tbVesselForobRawDatum);
                    }

                    await _repositoryWrapper.VesselForobRawData.CreateRangeAsync(tbVesselForobRawDatumList);
                    await _repositoryWrapper.SaveAync();

                    List<TbVesselForobAllocationRawDatum> TbVesselForobAllocationRawDatumList = new List<TbVesselForobAllocationRawDatum>();

                    foreach (var item in tbVesselForobRawDatumList)
                    {

                        foreach (var FuelUsage in reportForm.BunkerFuelUsages.Where(x => x.FuelName.Equals(item.FuelType, StringComparison.OrdinalIgnoreCase)))
                        {
                            TbVesselForobAllocationRawDatum tbVesselForobAllocationRawDatum = new TbVesselForobAllocationRawDatum();
                            tbVesselForobAllocationRawDatum.VesselForobRawDataId = item.Id;
                            tbVesselForobAllocationRawDatum.Name = FuelUsage.Name;
                            tbVesselForobAllocationRawDatum.Text = FuelUsage.Value.ToString();
                            TbVesselForobAllocationRawDatumList.Add(tbVesselForobAllocationRawDatum);
                        }
                    }

                    await _repositoryWrapper.VesselForobAllocationRawData.CreateRangeAsync(TbVesselForobAllocationRawDatumList);
                    await _repositoryWrapper.SaveAync();
                }
                if (reportForm.BunkerSteamsAndSurvey != null && reportForm.BunkerSteamsAndSurvey.Any())
                {
                    List<TbVesselBunkerStemsAndSurveysRawDatum> stemsAndSurveysRawDatas = new();
                    var count = 1;
                    for (int i = 0; i < reportForm.BunkerSteamsAndSurvey.Count; i++)
                    {
                        if ((reportForm.BunkerSteamsAndSurvey[i].ShipFigure != null
                            || reportForm.BunkerSteamsAndSurvey[i].BdnFigure != null)
                            && (reportForm.BunkerSteamsAndSurvey[i].SurveyDifferential != null
                            || reportForm.BunkerSteamsAndSurvey[i].DeBunkering != null))
                        {
                            TbVesselBunkerStemsAndSurveysRawDatum bunkerStemsAndSurveysRawData = new()
                            {
                                VesselRawDataId = tbVessel.Id,
                                Number = count++,
                                ShipsReceivedFigure = reportForm.BunkerSteamsAndSurvey[i].ShipFigure,
                                BdnFigure = reportForm.BunkerSteamsAndSurvey[i].BdnFigure,
                                StemOrSurvey = "Stem",
                                Grade = reportForm.BunkerSteamsAndSurvey[i].Key,
                            };
                            stemsAndSurveysRawDatas.Add(bunkerStemsAndSurveysRawData);
                            TbVesselBunkerStemsAndSurveysRawDatum bunkerStemsAndSurveysRawData1 = new()
                            {
                                VesselRawDataId = tbVessel.Id,
                                Number = count++,
                                SurveyRobDifferential = Convert.ToString(reportForm.BunkerSteamsAndSurvey[i].SurveyDifferential),
                                DebunkeredFigure = reportForm.BunkerSteamsAndSurvey[i].DeBunkering,
                                StemOrSurvey = "Survey",
                                Grade = reportForm.BunkerSteamsAndSurvey[i].Key
                            };
                            stemsAndSurveysRawDatas.Add(bunkerStemsAndSurveysRawData1);
                        }


                        else if ((reportForm.BunkerSteamsAndSurvey[i].ShipFigure != null || reportForm.BunkerSteamsAndSurvey[i].BdnFigure != null)
                            && reportForm.BunkerSteamsAndSurvey[i].SurveyDifferential == null
                            && reportForm.BunkerSteamsAndSurvey[i].DeBunkering == null)
                        {

                            TbVesselBunkerStemsAndSurveysRawDatum bunkerStemsAndSurveysRawData = new()
                            {
                                VesselRawDataId = tbVessel.Id,
                                Number = count++,
                                ShipsReceivedFigure = reportForm.BunkerSteamsAndSurvey[i].ShipFigure,
                                BdnFigure = reportForm.BunkerSteamsAndSurvey[i].BdnFigure,
                                StemOrSurvey = "Stem",
                                Grade = reportForm.BunkerSteamsAndSurvey[i].Key
                            };
                            stemsAndSurveysRawDatas.Add(bunkerStemsAndSurveysRawData);
                        }

                        else if ((reportForm.BunkerSteamsAndSurvey[i].SurveyDifferential != null || reportForm.BunkerSteamsAndSurvey[i].DeBunkering != null)
                            && reportForm.BunkerSteamsAndSurvey[i].ShipFigure == null
                            && reportForm.BunkerSteamsAndSurvey[i].BdnFigure == null)
                        {

                            TbVesselBunkerStemsAndSurveysRawDatum bunkerStemsAndSurveysRawData = new()
                            {
                                VesselRawDataId = tbVessel.Id,
                                Number = count++,
                                SurveyRobDifferential = Convert.ToString(reportForm.BunkerSteamsAndSurvey[i].SurveyDifferential),
                                DebunkeredFigure = reportForm.BunkerSteamsAndSurvey[i].DeBunkering,
                                StemOrSurvey = "Survey",
                                Grade = reportForm.BunkerSteamsAndSurvey[i].Key
                            };
                            stemsAndSurveysRawDatas.Add(bunkerStemsAndSurveysRawData);
                        }
                    }
                    await _repositoryWrapper.VesselBunkerStemsAndSurveysRawData.CreateRangeAsync(stemsAndSurveysRawDatas);
                }

                if (reportForm.BunkerTank != null && reportForm.BunkerTank.Any())
                {
                    List<TbVesselBunkerTankRawDatum> tbVesselBunkerList = new();
                    for (int k = 0; k < reportForm.BunkerTank.Count; k++)
                    {
                        var count = 1;
                        TbVesselBunkerTankRawDatum tbVesselBunker = new()
                        {
                            BunkerTankDescription = reportForm.BunkerTank[k].Description,
                            BunkerTankFuelGrade = Convert.ToDecimal(reportForm.BunkerTank[k].FuelGrade),
                            BunkerTankCapacity = reportForm.BunkerTank[k].Capacity,
                            BunkerTankObservedVolume = reportForm.BunkerTank[k].ObsVolume,
                            BunkerTankRob = reportForm.BunkerTank[k].Rob,
                            BunkerTankFillPercent = reportForm.BunkerTank[k].Fill,
                            BunkerTankSupplyDate = reportForm.BunkerTank[k].SupplyDate,
                            VesselDataRawId = tbVessel.Id,
                            BunkerTankNo = count++,
                            BunkerTankUnit = "m3"
                        };
                        tbVesselBunkerList.Add(tbVesselBunker);
                    }
                    await _repositoryWrapper.VesselBunkerTankRawData.CreateRangeAsync(tbVesselBunkerList);
                }

                await _repositoryWrapper.SaveAync();

                transaction.Commit();

                return tbVessel.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
                return 0;

            }

        }

    }


}
