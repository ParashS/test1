namespace Vessels.ApiModels
{
    public class CommonApiResponse
    {
        public int? statusCode { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
}
