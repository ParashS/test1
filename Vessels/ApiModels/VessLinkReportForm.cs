using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Vessels.ApiModels
{
    public class VessLinkModel 
    {
        public class VessLinkReportForm
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
            public string? Latitude { get; set; }
            public string? Longitude { get; set; }         
            public string? ReportTime { get; set; }
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
            public string? Port { get; set; }
            public string? ETD { get; set; }
           
            public string? Stoppages { get; set; }
            public string? Instructed_RPM { get; set; }
            public string? Remarks { get; set; }
            public string? Distance__Vessel_Explanation { get; set; }
            public string? Auxiliary_Engine_Instructions { get; set; }
            public string? Reason_to_omit_report_from_Performance { get; set; }
            public BunkerStemsAndSurveys? Bunker_Stems_and_Surveys { get; set; }
            public string? Bunker_Stem_and_Survey_Description { get; set; }
            public string? Bunker_Tank_Instructions { get; set; }
            public string? Bunker_Rob_Instructions { get; set; }
            public string? Reason_for_Other_Consumption1 { get; set; }
            public Upcoming_Ports? Upcoming { get; set; }

        }
        public class PositionReportModel : VessLinkReportForm
        {
            public string? Event { get; set; }
            public string? Location { get; set; }
        }
        public class DepartureReportModel : VessLinkReportForm
        {
            public string? Arrival_Position { get; set; }
            public string? SWBWFW { get; set; }
            public string? SWBWFW_Est { get; set; }
            public decimal? Est_Fwd_Draft { get; set; }
            public decimal? Est_Aft_Draft { get; set; }
            public decimal? Total_Cargo_on_Board { get; set; }
            public string? Cargo_Grade { get; set; }
            public decimal? FreshWaterMade { get; set; }
            public string? SlopsROB { get; set; }
            public string? Slops__Annex_1_or_Annex_2 { get; set; }
            public string? Grade_of_Slops { get; set; }
            public string? Slop_Stowage { get; set; }

            public string? Inspections_In_Port_1 { get; set; }
            public string? Inspections_In_Port_2 { get; set; }
            public string? Inspections_In_Port_3 { get; set; }
            public string? Inspect_Co_1 { get; set; }
            public string? Inspect_Co_2 { get; set; }
            public string? Inspect_Co_3 { get; set; }
            public string? Date_1 { get; set; }
            public string? Date_2 { get; set; }
            public string? Date_3 { get; set; }
            public string? Number_of_Tugs_In { get; set; }
            public string? Tug_In_Start_Time { get; set; }
            public string? Number_of_Tugs_Out { get; set; }
            public string? Tugs_Out_Start_Time { get; set; }
            public string? Number_of_Shifting_Tugs { get; set; }
            public string? Shifting_Tugs_Start_Time { get; set; }
            public string? Berth_Shifted_To__In { get; set; }
            public string? Tugs_in_End_Time { get; set; }
            public string? Berth_Shifted_From { get; set; }
            public string? Tugs_Out_End_Time { get; set; }
            public string? Berth_Shifted_To { get; set; }
            public string? Shifting_Tugs_End_Time { get; set; }
            public string? PEId { get; set; }
        }
        public class ArrivalReportModel : VessLinkReportForm
        {
            public string? Arrival_Position { get; set; }
            public string? NOR_Tendered_Time { get; set; }
            public string? Time_Drop_of_Anchor { get; set; }
            public string? Free_Pratique_Granted { get; set; }
            public string? ETB { get; set; }
            public string? LOP_issued { get; set; }
            public string? SlopsROB { get; set; }
            public string? Slops__Annex_1_or_Annex_2 { get; set; }
            public string? Grade_of_Slops { get; set; }
            public string? Slop_Stowage { get; set; }
            public string? PSId { get; set; }

        }
        public class DisplayObj
        {
            public string? Key { get; set; }
            public dynamic? Value { get; set; }
        }
    }
}
