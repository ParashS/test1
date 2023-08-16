using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessel_Forob_AllocationRawData")]
    public partial class TbVesselForobAllocationRawDatum
    {
        [Key]
        public long Id { get; set; }
        [Column("Vessel_Forob_RawDataId")]
        public long VesselForobRawDataId { get; set; }
        [StringLength(512)]
        public string? Name { get; set; }
        [StringLength(512)]
        public string? Text { get; set; }

        [ForeignKey("VesselForobRawDataId")]
        [InverseProperty("TbVesselForobAllocationRawData")]
        public virtual TbVesselForobRawDatum VesselForobRawData { get; set; } = null!;
    }
}
