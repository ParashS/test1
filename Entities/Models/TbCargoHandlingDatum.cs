using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_CargoHandling")]
    public partial class TbCargoHandlingDatum
    {
        [Key]
        public long CargoId { get; set; }
        public long? VesselDataRawId { get; set; }
        public int? CargoTypeId { get; set; }
        public string? Function { get; set; }   
        public DateTime? BLDate { get; set; }
        public string? BLCode { get; set; }
        public decimal? BLGross { get; set; }
        public string? CargoName { get; set; }
        public string? Berth { get; set; }
        public decimal? ShipGross { get; set; }
        public decimal? LoadTemp { get; set; }
        public decimal? ApiGravity { get; set; }
        public string? UnitCode { get; set; }
        public string? Charterer { get; set; }
        public string? Consignee { get; set; }
        public string? Receiver { get; set; }
        public string? Shipper { get; set; }
        public string? Destination { get; set; } 
        public string? LetterOfProtest { get; set; }
        [ForeignKey("VesselDataRawId")]
        [InverseProperty("TbCargoHandlingData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
        [InverseProperty("CargoHandlingData")]
        public virtual ICollection<TbStowagesDatum> TbStowagesData { get; set; } = null!;

    }
}
