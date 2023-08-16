using Newtonsoft.Json;

namespace Vessels.ApiModels
{
    public partial class ArrivalReportResponse
    {
        public Form? Form { get; set; }
    }

    public partial class Allocation
    {
        [JsonProperty("@Name")]
        public string? Name { get; set; }

        [JsonProperty("#text")]
        public string? text { get; set; }
    }

    public partial class AuxilliaryEngineHours
    {
        public dynamic? Auxilliary_Engine_HoursRow { get; set; }
    }

    public partial class AuxilliaryEngineHoursRow
    {
        public string? Aux_Engine { get; set; }
        public decimal? Hours { get; set; }
    }

    public partial class BunkerTanks
    {
        public dynamic? BunkerTanksRow { get; set; }
        //public List<BunkerTanksRow>? BunkerTanksRow { get; set; }
    }

    public partial class BunkerTanksRow
    {
        public int? BunkerTankNo { get; set; }
        public string? BunkerTankDescription { get; set; }
        public decimal? BunkerTankFuelGrade { get; set; }
        public decimal? BunkerTankCapacity { get; set; }
        public decimal? BunkerTankObservedVolume { get; set; }
        public string? BunkerTankUnit { get; set; }
        public decimal? BunkerTankROB { get; set; }
        public decimal? BunkerTankFillPercent { get; set; }
        public string? BunkerTankSupplyDate { get; set; }
    }

    public partial class Form
    {
        [JsonProperty("@FormIdentifier")]
        public string? FormIdentifier { get; set; }

        [JsonProperty("@CompanyCode")]
        public string? CompanyCode { get; set; }

        [JsonProperty("@CompanyName")]
        public string? CompanyName { get; set; }

        [JsonProperty("@VesselCode")]
        public string? VesselCode { get; set; }

        [JsonProperty("@SubmittedDate")]
        public DateTime? SubmittedDate { get; set; }

        [JsonProperty("@Status")]
        public string? Status { get; set; }

        [JsonProperty("@ApprovedDate")]
        public DateTime? ApprovedDate { get; set; }

        [JsonProperty("@LatestResubmissionDate")]
        public DateTime? LatestResubmissionDate { get; set; }

        [JsonProperty("@FormGUID")]
        public string? FormGUID { get; set; }

        [JsonProperty("@ImoNumber")]
        public string? ImoNumber { get; set; }

        [JsonProperty("@UnCode")]
        public string? UnCode { get; set; }
        public string? VesselName { get; set; }
        public int? VoyageNo { get; set; }
        public string? VesselCondition { get; set; }
        public decimal? FWDDraft { get; set; }
        public decimal? AFTDraft { get; set; }
        public string? Arrival_Position { get; set; }
        public string? Port { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ReportTime { get; set; }
        public string? Time_Drop_of_Anchor { get; set; }
        public decimal? SteamingHrs { get; set; }
        public decimal? MainEngineHrs { get; set; }
        public decimal? ObservedDistance { get; set; }
        public decimal? EngineDistance { get; set; }
        public decimal? Logged_Distance { get; set; }
        public decimal? ObsSpeed { get; set; }
        public decimal? CPSpeed { get; set; }
        public decimal? Logged_Speed { get; set; }
        public decimal? RPM { get; set; }
        public decimal? MEOutputPct { get; set; }
        public decimal? Slip { get; set; }
        public decimal? WindForce { get; set; }
        public string? WindDirection { get; set; }
        public FOROB? FOROB { get; set; }
        public decimal? Fresh_Water_Produced_since_last_report_Mts { get; set; }
        public decimal? FreshWaterROB { get; set; }
        public decimal? Fresh_Water_for_Tank_Cleaning { get; set; }
        public AuxilliaryEngineHours? Auxilliary_Engine_Hours { get; set; }
        public BunkerTanks? BunkerTanks { get; set; }
        public string? ETD { get; set; }
        public string? NOR_Tendered_Time { get; set; }
        public string? LOP_issued { get; set; }
        public string? Free_Pratique_Granted { get; set; }
        public string? ETB { get; set; }
        public string? Stoppages { get; set; }
        public string? Instructed_RPM { get; set; }
        public string? Remarks { get; set; }
        public string? Distance__Vessel_Explanation { get; set; }
        public string? Auxiliary_Engine_Instructions { get; set; }
        public string? Reason_to_omit_report_from_Performance { get; set; }
        public string? SlopsROB { get; set; }
        public string? Slops__Annex_1_or_Annex_2 { get; set; }
        public string? Grade_of_Slops { get; set; }
        public string? Slop_Stowage { get; set; }
        public string? Bunker_Tank_Instructions { get; set; }
        public string? Bunker_Rob_Instructions { get; set; }
        public string? Reason_for_Other_Consumption1 { get; set; }
        public string? __PSId { get; set; }
    }

    public partial class FOROB
    {
        public Robs? Robs { get; set; }
    }

    public partial class Rob
    {
        [JsonProperty("@FuelType")]
        public string? FuelType { get; set; }

        [JsonProperty("@Remaining")]
        public decimal? Remaining { get; set; }

        [JsonProperty("@AuxEngineConsumption")]
        public decimal? AuxEngineConsumption { get; set; }

        [JsonProperty("@BoilerEngineConsumption")]
        public decimal? BoilerEngineConsumption { get; set; }

        [JsonProperty("@Units")]
        public string? Units { get; set; }

        [JsonProperty("@Received")]
        public decimal? Received { get; set; }

        [JsonProperty("@Consumption")]
        public decimal? Consumption { get; set; }
        
        [JsonProperty("Allocation")]
        public dynamic? Allocation { get; set; }
        
    }

    public partial class Robs
    {
        [JsonProperty("@AsOfDate")]
        public string? AsOfDate { get; set; }
        public List<Rob>? Rob { get; set; }
    }
}
