using Newtonsoft.Json;

namespace Vessels.ApiModels
{
    public partial class DepartureReportResponse
    {
        public Form? Form { get; set; }
    }

    public partial class BunkerStemsAndSurveys
    {
        //public BunkerStemsAndSurveysRow? Bunker_Stems_and_SurveysRow { get; set; }
        public dynamic? Bunker_Stems_and_SurveysRow { get; set; }
    }

    public partial class BunkerStemsAndSurveysRow
    {
        public int? Number { get; set; }
        public string? Grade { get; set; }
        public string? Stem_or_Survey { get; set; }
        public decimal? BDN_Figure { get; set; }
        public decimal? Ships_Received_Figure { get; set; }
        public string? Survey_ROB_Differential { get; set; }
    }

    public partial class Form
    {
        public Upcoming_Ports? Upcoming { get; set; }
        public int? DistanceToGo { get; set; }
        public decimal? Est_Fwd_Draft { get; set; }
        public decimal? Est_Aft_Draft { get; set; }
        public string? SWBWFW { get; set; }
        public string? SWBWFW_Est { get; set; }
        public decimal? Total_Cargo_on_Board { get; set; }
        public string? Cargo_Grade { get; set; }
        public decimal? FreshWaterMade { get; set; }
        public BunkerStemsAndSurveys? Bunker_Stems_and_Surveys { get; set; }
        public string? Bunker_Stem_and_Survey_Description { get; set; }
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
        public string? __PEId { get; set; }
    }

    public partial class Upcoming_Ports
    {
        public dynamic? UpcomingPort { get; set; }
    }

    public partial class UpcomingPort
    {
        [JsonProperty("@PortName")]
        public string? PortName { get; set; }

        [JsonProperty("@DistToGo")]
        public decimal? DistToGo { get; set; }

        [JsonProperty("@ProjSpeed")]
        public decimal? ProjSpeed { get; set; }

        [JsonProperty("@ETA")]
        public string? ETA { get; set; }

        [JsonProperty("@Via")]
        public string? Via { get; set; }

        [JsonProperty("@UnCode")]
        public string? UnCode { get; set; }
    }
}
