using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class GetVersionNumberByReportRequestDto
    {
        public string? FormIdentifier { get; set; }
        public DateTime? ReportTime { get; set; }
        public DateTime? LatestResubmissionDate { get; set; }
        public string? ImoNumber { get; set; }
        public string? ReportType { get; set; }
    }
}
