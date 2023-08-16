using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessel_ForobRawData")]
    public partial class TbVesselForobRawDatum
    {
        public TbVesselForobRawDatum()
        {
            TbVesselForobAllocationRawData = new HashSet<TbVesselForobAllocationRawDatum>();
        }

        [Key]
        public long Id { get; set; }
        public long VesselDataRawId { get; set; }
        [StringLength(512)]
        public string? FuelType { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? Remaining { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? AuxEngineConsumption { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BoilerEngineConsumption { get; set; }
        [StringLength(512)]
        public string? Units { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? Received { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? Consumption { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? RobError { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ROBDifference { get; set; }

        [ForeignKey("VesselDataRawId")]
        [InverseProperty("TbVesselForobRawData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
        [InverseProperty("VesselForobRawData")]
        public virtual ICollection<TbVesselForobAllocationRawDatum> TbVesselForobAllocationRawData { get; set; }
    }
}
