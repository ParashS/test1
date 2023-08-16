using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessel_UpcomingPortRawData")]
    public partial class TbVesselUpcomingPortRawDatum
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        public long VesselDataRawId { get; set; }
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

        [ForeignKey("VesselDataRawId")]
        [InverseProperty("TbVesselUpcomingPortRawData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
    }
}
