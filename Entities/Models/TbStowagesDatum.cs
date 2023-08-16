using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Stowage")]
    public partial class TbStowagesDatum
    {
        [Key]
        public long Id { get; set; }
        public long? CargoId { get; set; }
        public string? TankName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }

        //[ForeignKey("CargoId")]
        //[InverseProperty("tb_Stowage")]
        //public virtual TbCargoHandlingDatum CargoHandlingDatum { get; set; } = null!;
        [ForeignKey("CargoId")]
        [InverseProperty("TbStowagesData")]
        public virtual TbCargoHandlingDatum CargoHandlingData { get; set; } = null!;

    }
}
