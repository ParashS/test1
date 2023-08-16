using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace Vessels.ApiModels
{
    public class CargoHandlingReportResponse
    {
        public Form? Form { get; set; }

    }
    public partial class Form
    {
        public Cargoes_model? Cargo { get; set; }
    }
    public partial class Cargoes_model
    {
        //public List<Cargo>? Cargoes { get; set; }
        public dynamic? Cargoes { get; set; }
    }

    public partial class Cargo
    {
        [JsonProperty("@CargoTypeID")]
        public int? CargoTypeID { get; set; }

        [JsonProperty("@Function")]
        public string? Function { get; set; }
        [JsonProperty("@Berth")]
        public string? Berth { get; set; }

        [JsonProperty("@BLDate")]
        public DateTime? BLDate { get; set; }

        [JsonProperty("@BLGross")]
        public decimal? BLGross { get; set; }
        [JsonProperty("@BLCode")]
        public string? BLCode{ get; set; }

        [JsonProperty("@CargoName")]
        public string? CargoName { get; set; }

        [JsonProperty("@ShipGross")]
        public decimal? ShipGross { get; set; }

        [JsonProperty("@LoadTemp")]
        public decimal? LoadTemp { get; set; }

        [JsonProperty("@APIGravity")]
        public decimal? APIGravity { get; set; }

        [JsonProperty("@UnitCode")]
        public string? UnitCode { get; set; }

        [JsonProperty("@Charterer")]
        public string? Charterer { get; set; }

        [JsonProperty("@Consignee")]
        public string? Consignee { get; set; }

        [JsonProperty("@Receiver")]
        public string? Receiver { get; set; }

        [JsonProperty("@Shipper")]
        public string? Shipper { get; set; }

        [JsonProperty("@Destination")]
        public string? Destination { get; set; }

        [JsonProperty("@LetterOfProtest")]
        public string? LetterOfProtest { get; set; }

        public List<Stowage>? Stowage { get; set; }
    }
    public partial class Stowage
    {
        public string? TankName { get; set; }
        public decimal? Quantity{ get; set; }
        public string? Unit { get; set; }

    }


}