using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class UpcomingDto
    {
        [StringLength(50)]
        public string? PortName { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? DistToGo { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ProjSpeed { get; set; }
        [Column("ETA")]
        [StringLength(50)]
        public string? Eta { get; set; }
        [StringLength(50)]
        public string? Via { get; set; }
        [StringLength(50)]
        public string? UnCode { get; set; }
    }
}
