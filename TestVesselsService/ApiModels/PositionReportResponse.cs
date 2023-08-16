using Newtonsoft.Json;

namespace TestVesselsService.ApiModels
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
