using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class AuxilliaryEngineHoursDto
    {
        public long?  VesselRawDtoId { get; set; }
        public string? Aux_Engine { get; set; }
        public decimal? Hours { get; set; }
    }
}
