using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class RobDto
    {
        public long? VesselFobId { get; set; }
        public long? VesselDataRawId { get; set; }
        public string? FuelType { get; set; }
        public decimal? Remaining { get; set; }
        public decimal? AuxEngineConsumption { get; set; }
        public decimal? BoilerEngineConsumption { get; set; }
        public string? Units { get; set; }
        public decimal? Received { get; set; }
        public decimal? Consumption { get; set; }

        public List<AllocationDto>? Allocation { get; set; }
    }
}
