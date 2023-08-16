using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessels_AuxilliaryRawData")]
    public partial class TbVesselsAuxilliaryRawDatum
    {
        [Key]
        public long Id { get; set; }
        public long VesselDataRawId { get; set; }
        [Column("Aux_Engine")]
        public string? AuxEngine { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? Hours { get; set; }

        [ForeignKey("VesselDataRawId")]
        [InverseProperty("TbVesselsAuxilliaryRawData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
    }
}
