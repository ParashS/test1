using Newtonsoft.Json;

namespace Vessels.ApiModels
{
    public partial class PositionReportResponse
    {
        public Form? Form { get; set; }
    }

    public partial class Form
    {
        public string? Location { get; set; }
        public string? Event { get; set; }
    }
}
