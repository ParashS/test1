using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class BunkerTanksDto
    {
        public long? VesselRawDataId { get; set; }
        public int? BunkerTankNo { get; set; }
        public string? BunkerTankDescription { get; set; }
        public decimal? BunkerTankFuelGrade { get; set; }
        public decimal? BunkerTankCapacity { get; set; }
        public decimal? BunkerTankObservedVolume { get; set; }
        public string? BunkerTankUnit { get; set; }
        public decimal? BunkerTankROB { get; set; }
        public decimal? BunkerTankFillPercent { get; set; }
        public string? BunkerTankSupplyDate { get; set; }
    }
}
