using Newtonsoft.Json;

namespace Vessels.ApiModels
{
    public class StatementsOfFactsReportResponse
    {
        public Form? Form { get; set; }      
    }
    public partial class Form
    {
        public string? GmtOffset { get; set; }    
       public dynamic? PortActivities { get; set; }
    }

    public partial class Activity
    {
        //public int? ActivityId { get; set; }
        [JsonProperty("@Name")]
        public string? ActivityName { get; set; }
        [JsonProperty("@Time")]
        public DateTime? Time { get; set; }
        [JsonProperty("@CargoName")]
        public string? CargoName { get; set; }
        [JsonProperty("@Charterer")]
        public string? Charterer { get; set; }
        [JsonProperty("@Remark")]
        public string? Remarks { get; set; }
        [JsonProperty("@Berth")]
        public string? Berth { get; set;}
        
    }
}
