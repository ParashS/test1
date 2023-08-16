using System.ComponentModel.DataAnnotations;

namespace DataTransferObjects.ReportForm
{
    public class ReportFormDto
    {
        [Required]
        [RegularExpression("^Departure$|^Position$|^Arrival$|^CargoHandling$", ErrorMessage = "Please enter valid ReportType value.")]
        public string ReportType { get; set; }
        public string? ReasonForOtherConsumption { get; set; }
        public ReportOverView? ReportOverView { get; set; }
        public BasicDetails? BasicDetails { get; set; }
        public Operational? Operational { get; set; }
        public DistanceAndSpeed? DistanceAndSpeed { get; set; }
        public AvgEngineParameter? AvgEngineParameter { get; set; }
        public Weather? Weather { get; set; }
        public FreshWater? FreshWater { get; set; }
        public Officers? Officers { get; set; }
        public PortSof? PortSof { get; set; }
        public Slops? Slops { get; set; }
        public Inspection? Inspection { get; set; }
        public Tugs? Tugs { get; set; }
        public List<UpcommingPort>? UpcommingPorts { get; set; }
        public List<AuxEngine>? AuxEngine { get; set; }
        public List<string>? FuelTypes { get; set; }
        public List<BunkerFuelUsages>? BunkerFuelUsages { get; set; }
        public List<BunkerSteamsAndSurvey>? BunkerSteamsAndSurvey { get; set; }
        public List<BunkerTank>? BunkerTank { get; set; }
        public Cargo? Cargo { get; set; }

    }

    public class BasicDetails
    {
        [Required]
        public string? VesselName { get; set; }
        [RegularExpression(@"^\d{7}$" ,ErrorMessage = "Please enter correct IMO number")]
        public int? VesselImo { get; set; }
        public string? Location { get; set; }
        public string? ReportEvent { get; set; }
        public string? VoyageNo { get; set; }
    }

    public class ReportOverView
    {
        [Required(ErrorMessage ="Please enter correct date & time")]
        public DateTime LocalDateTime { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}[+-]\d{2}:\d{2}$", ErrorMessage ="Please enter correct local Datetime")]
        public string? LocalDateTimeString { get; set; }
        [Required]
        [RegularExpression(@"^([0-8]?[0-9])\s+([0-5]?[0-9])'\s+(([0-5]?[0-9])(?:\.\d{1,20})?)""\s+([NnSs])$", ErrorMessage = "Please enter right latitude")]
        public string Latitude { get; set; }
        [Required]
        [RegularExpression(@"^([0-9]{1,2}|1[0-7][0-9]|180)\s+([0-5]?[0-9])'\s+(([0-5]?[0-9])(?:\.\d{1,20})?)""\s+([EeWw])$", ErrorMessage = "Please enter right latitude")]
        public string Longitude { get; set; }
        public string? CurrentPort { get; set; }
        public string? PortEtd { get; set; }
        public string? SwbwfwEst { get; set; }
        public decimal? EstAftDraft { get; set; }
        public decimal? EstFwdDraft { get; set; }
    }

    public class Operational
    {
        [Required]
        [RegularExpression("^Laden$|^Ballast$", ErrorMessage = "Please enter valid Vessel condition value.")]
        public string? VesselCondition { get; set; }
        public decimal? Fwddraft { get; set; }
        public decimal? Aftdraft { get; set; }
        public string? Remarks { get; set; }
        public string? SWBWFW { get; set; }
        

    }

    public class DistanceAndSpeed
    {
        public decimal? SteamingHours { get; set; }
        public decimal? Stoppages { get; set; }
        public decimal? InstructedSpeed { get; set; }
        public decimal? InstructedRPM { get; set; }
        public decimal? ObservedDistance { get; set; }
        public decimal? LoggedDistance { get; set; }
        public decimal? EngineDistance { get; set; }
        public decimal? SpeedOverGround { get; set; }
        public decimal? SpeedThroughWater { get; set; }
        public decimal? ManoeuvringHrs { get; set; }

    }

    public class AvgEngineParameter
    {
        public decimal? RPM { get; set; }
        public decimal? MainEngineLoadOutput { get; set; }
        public decimal? MainEnginePower { get; set; }
        public decimal? Slip { get; set; }
    }

    public class Weather
    {
        public string? WindDirection { get; set; }
        public decimal? BeaufortForce { get; set; }
        public decimal? DSS { get; set; }
        public string? ReasonToOmitReportFromPerformance { get; set; }
    }

    public class FreshWater
    {
        public decimal? FreshWaterROB { get; set; }
        public decimal? FreshWaterProduced { get; set; }
        public decimal? FreshWaterForTankCleaning { get; set; }
        public decimal? FreshWaterRecieved { get; set; }

    }

    public class Officers
    {
        public string? MasterName { get; set; }
        public string? MasterSurname { get; set; }
        public string? ChiefEngineerName { get; set; }
        public string? ChiefEngineerSurname { get; set; }
    }

    public class PortSof
    {
        public string? NorTenderedTime { get; set; }
        public string? FreePartiqueGranted { get; set; }
        public string? TimeDropOfAnchor { get; set; }
        public string? LopIssued { get; set; }
        public string? EstimatedBerthInTime { get; set; }
    }
    public class Slops
    {
        public decimal? SlopsRob { get; set; }
        public string? GradeOfSlops { get; set; }
        public string? SlopsAnnex1OrAnnex2 { get; set; }
        public string? SlopStowage { get; set; }
    }
    public class Inspection
    {
        public string? InspectCo1 { get; set; }
        public string? InspectCo2 { get; set; }
        public string? InspectCo3 { get; set; }
        public string? InspectionsInPort1 { get; set; }
        public string? InspectionsInPort2 { get; set; }
        public string? InspectionsInPort3 { get; set; }
        public string? Date1 { get; set; }
        public string? Date2 { get; set; }
        public string? Date3 { get; set; }
    }

    public class Tugs
    {
        public decimal? NumberOfTugsIn { get; set; }
        public string? TugInStartTime { get; set; }
        public string? TugsInEndTime { get; set; }
        public decimal? NumberOfTugsOut { get; set; }
        public string? TugsOutStartTime { get; set; }
        public string? TugsOutEndTime { get; set; }
        public decimal? NumberOfShiftingTugs { get; set; }
        public string? BerthShiftedTo { get; set; }
        public string? ShiftingTugsStartTime { get; set; }
        public string? ShiftingTugsEndTime { get; set; }
        public string? FirstBerthIn { get; set; }
        public string? LastBerthOut { get; set; }


    }
    public class UpcommingPort
    {
        public string? PortName { get; set; }
        public string? Via { get; set; }
        public string? PortEta { get; set; }
        public decimal? DistToGo { get; set; }
        public decimal? ProjSpeed { get; set; }
    }


    public class AuxEngine
    {
        public string? Name { get; set; }
        public decimal? Hours { get; set; }
    }
    public class BunkerFuelUsages
    {
        public string? FuelName { get; set; }
        public string? Section { get; set; }
        public string? Name { get; set; }
        public decimal? Value { get; set; }

    }

    public class BunkerSteamsAndSurvey
    {
        public string? Key { get; set; }
        public decimal? ShipFigure { get; set; }
        public decimal? BdnFigure { get; set; }
        public decimal? SurveyDifferential { get; set; }
        public decimal? DeBunkering { get; set; }
    }

    public class BunkerTank
    {
        public string? Description { get; set; }
        public string? FuelGrade { get; set; }
        public decimal? Capacity { get; set; }
        public decimal? ObsVolume { get; set; }
        public decimal? Rob { get; set; }
        public decimal? Fill { get; set; }
        public string? SupplyDate { get; set; }
    }

    public class Cargo
    {
        public decimal? TotalCargoOnBoard { get; set; }
        public string? CargoGrade { get; set; }
        public decimal? CargoDischarge { get; set; }
        public decimal? CargoLoaded { get; set; }
    }
}
