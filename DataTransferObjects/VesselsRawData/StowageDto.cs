using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class StowageDto
    {
        public long? StowageId { get; set; }
        public long? CargoId { get; set; }
        public string? TankName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
