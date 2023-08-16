namespace Services.Helpers
{
  public static class StaticData
  {
    public static Dictionary<string, ReportTypes> ReportTypeDictionary = new Dictionary<string, ReportTypes>()
    {
      { "COSP Report Navig8 S5 API (NAVG)", ReportTypes.Departure },
      { "Noon Position Report Navig8 S5 API (NAVG)", ReportTypes.Position },
      { "EOSP Report Navig8 S5 API (NAVG)", ReportTypes.Arrival },
      { "Statement of Facts Navig8 S5 (NAVG)", ReportTypes.StatementOfFacts },
      { "Cargo Handling Form S5 (NAVG)", ReportTypes.CargoHandling }
    };

    public static Dictionary<string, string> ReportTypeNames = new Dictionary<string, string>()
    {
      { "COSP Report Navig8 S5 API (NAVG)", "Departure" },
      { "Noon Position Report Navig8 S5 API (NAVG)", "Position" },
      { "EOSP Report Navig8 S5 API (NAVG)", "Arrival" },
      { "Statement of Facts Navig8 S5 (NAVG)", "StatementOfFacts" },
      { "Cargo Handling Form S5 (NAVG)", "CargoHandling" }
    };
  }

  public enum ReportTypes
  {
    Departure,
    Position,
    Arrival,
    StatementOfFacts,
    CargoHandling
  }
}
