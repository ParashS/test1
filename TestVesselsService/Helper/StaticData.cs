namespace TestVesselsService.Helper
{
    public static class StaticData
    {
        public static Dictionary<ReportTypes, string> ReportTypeDictionary = new Dictionary<ReportTypes, string>()
        {
            { ReportTypes.Departure, "COSP Report Navig8 S5 API (NAVG)" },
            { ReportTypes.Position, "Noon Position Report Navig8 S5 API (NAVG)" },
            { ReportTypes.Arrival, "EOSP Report Navig8 S5 API (NAVG)" }
        };

    }

    public enum ReportTypes
    {
        Departure,
        Position,
        Arrival
    }
}
