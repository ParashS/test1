using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class PortActivitiesDto
    {
        public long ActivityID { get; set; }    
        public long? VesselRawDataId { get; set; }    
        public string? ActivityName { get; set; }    
        public DateTime? Time { get; set; }    
        public string? CargoName { get; set; }    
        public string? Charterer { get; set; }    
        public string? Remarks { get; set; }    
        public string? Berth { get; set; }    
    }
}
