using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessel_BunkerTankRawData")]
    public partial class TbVesselBunkerTankRawDatum
    {
        [Key]
        public long Id { get; set; }
        public long VesselDataRawId { get; set; }
        public int? BunkerTankNo { get; set; }
        [Unicode(false)]
        public string? BunkerTankDescription { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BunkerTankFuelGrade { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BunkerTankCapacity { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BunkerTankObservedVolume { get; set; }
        [Unicode(false)]
        public string? BunkerTankUnit { get; set; }
        [Column("BunkerTankROB", TypeName = "decimal(27, 20)")]
        public decimal? BunkerTankRob { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BunkerTankFillPercent { get; set; }
        [StringLength(50)]
        public string? BunkerTankSupplyDate { get; set; }

        [ForeignKey("VesselDataRawId")]
        [InverseProperty("TbVesselBunkerTankRawData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
    }
}
