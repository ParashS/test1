namespace Vessels.Helper
{
  public static class StaticData
  {
    public static Dictionary<ReportTypes, string> ReportTypeDictionary = new Dictionary<ReportTypes, string>()
    {
       { ReportTypes.Departure, "COSP Report Navig8 S5 API (NAVG)" },
       { ReportTypes.Position, "Noon Position Report Navig8 S5 API (NAVG)" },
       { ReportTypes.Arrival, "EOSP Report Navig8 S5 API (NAVG)" }
    };

    public const string DATE_TIME_ZONE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";
  }

    public enum ReportTypes
    {
        Departure,
        Position,
        Arrival
    }
    public enum FuelTypes
    {
        LFO, LGO, MGO, IFO
    }

    public enum VesselConditions
    {
        Ballast,
        Laden
    }
}
