using Irony.Parsing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Vessels.Helper
{
    public static class ValidateReportData
    {
        public static bool ValidateData(string dataType, string data)
        {
            try
            {
                switch (dataType)
                {
                    case "Decimal":
                        Convert.ToDecimal(data);
                        break;
                    case "Integer":
                        Convert.ToInt32(data);
                        break;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>Validates the vessel imo number.</summary>
        /// <param name="vesselImoNumber">The vessel imo number.</param>
        /// <param name="imoNumberList">The imo number list that comes from database.</param>
        /// <returns>
        /// valid or invalid imo number
        /// </returns>
        /// 
        public static bool ValidateVesselImoNumber(string? vesselImoNumber, IEnumerable<string?> imoNumberList, out bool isInActiveVesselList)
        {
            bool isImoNumberValid = false;
            isInActiveVesselList = imoNumberList.Contains(vesselImoNumber);

            if (!string.IsNullOrWhiteSpace(vesselImoNumber) && isInActiveVesselList)
            {
                Regex regexForImoNumber = new Regex("^[0-9]{0,7}$");
                isImoNumberValid = regexForImoNumber.IsMatch(vesselImoNumber);
            }

            return isImoNumberValid;
        }

        /// <summary>Validates the form identifier.</summary>
        /// <param name="formIdentifier">The form identifier.</param>
        /// <returns>
        ///   valid or invalid form identifier
        /// </returns>
        public static bool ValidateFormIdentifier(string? formIdentifier)
        {
            bool isFormIdentifierValid = false;

            if (!string.IsNullOrWhiteSpace(formIdentifier) && Services.Helpers.StaticData.ReportTypeNames.ContainsKey(formIdentifier))
                isFormIdentifierValid = true;

            return isFormIdentifierValid;
        }

        /// <summary>Gets the date time from offset.</summary>
        /// <param name="dateString">The date string.</param>
        /// <returns>
        ///   valid converted date time formatted date time or null
        /// </returns>
        public static DateTime? GetDateTimeFromOffset(string? dateString)
        {
            DateTime dateTime;

            if (!string.IsNullOrWhiteSpace(dateString) &&
                DateTime.TryParseExact(dateString, StaticData.DATE_TIME_ZONE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                string? offSet = string.IsNullOrWhiteSpace(dateString) ? null : dateString[^6..];   // [^6..] means [dateString.Length - 6]

                if (offSet != null && (offSet[0] == '+' || offSet[0] == '-'))
                {
                    return Convert.ToDateTime(dateString).ToUniversalTime();
                }

                return dateTime;
            }

            return null;
        }

        /// <summary>Gets the report location.</summary>
        /// <param name="location">The location.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns>
        ///   report location as per the conditions or empty
        /// </returns>
        public static string? GetReportLocation(string? location, string reportType)
        {
            return location == "N" && reportType == "Position" ? "At Sea" :
                   location == "S" && reportType == "Position" ? "In Port" :
                   location == "W" && reportType == "Position" ? "In Port" :
                   string.IsNullOrWhiteSpace(location) && reportType == "Position" ? "Drifting" :
                   string.Empty;
        }

        /// <summary>Determines whether [is report latitute or longitude valid] [the specified latitude or longitude].</summary>
        /// <param name="latitudeOrLongitude">The latitude or longitude.</param>
        /// <returns>
        ///   <c>true</c> if [is report latitute or longitude valid] [the specified latitude or longitude]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReportLatituteOrLongitudeValid(string? latitudeOrLongitude, bool isLong = true)
        {
            bool isLatitudeOrLongitudeValid = false;

            if (!string.IsNullOrWhiteSpace(latitudeOrLongitude))
            {
                //Longitude: https://regexr.com/5lcpk
                //Latitude: https://regexr.com/5lcq9

                string regexString = isLong ? @"^([0-9]{1,2}|1[0-7][0-9]|180)\s+([0-5]?[0-9])'\s+(([0-5]?[0-9])(?:\.\d{1,20})?)""\s+([EeWw])$" :
                                              @"^([0-8]?[0-9])\s+([0-5]?[0-9])'\s+(([0-5]?[0-9])(?:\.\d{1,20})?)""\s+([NnSs])$";

                Regex regex = new Regex(regexString);

                return regex.IsMatch(latitudeOrLongitude);
            }

            return isLatitudeOrLongitudeValid;
        }

        /// <summary>Determines whether [is vessel condition valid] [the specified vessel condition].</summary>
        /// <param name="vesselCondition">The vessel condition.</param>
        /// <returns>
        ///   <c>true</c> if [is vessel condition valid] [the specified vessel condition]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVesselConditionValid(string? vesselCondition)
        {
            bool isVesselConditionValid = false;

            if (!string.IsNullOrWhiteSpace(vesselCondition) && Enum.IsDefined(typeof(VesselConditions), vesselCondition))
            {
                isVesselConditionValid = true;
            }

            return isVesselConditionValid;
        }

        /// <summary>Converts the date time to universal time or return null.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        ///  Converted date time to universal time or null
        /// </returns>
        public static DateTime? ConvertDateTimeToUniversalTimeOrReturnNull(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return dateTime.Value.ToUniversalTime();
            }

            return dateTime;
        }

        /// <summary>Fuels the type has valid value.</summary>
        /// <param name="fuelType">Type of the fuel.</param>
        /// <returns>
        ///   return the valid value by checking it to the predefined FuelTypes or null
        /// </returns>
        public static string? FuelTypeHasValidValue(string? fuelType)
        {
            if (!string.IsNullOrWhiteSpace(fuelType) && Enum.IsDefined(typeof(FuelTypes), fuelType))
            {
                return fuelType;
            }

            return null;
        }
    }
}
