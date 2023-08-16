using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_PortActivities")]
    public partial class TbPortActivities
    {
        [Key]
        public long ActivityID { get; set; }
        public long? VesselDataRawID { get; set; }
        public string? ActivityName { get; set; }
        public DateTime? Time { get; set; }
        public string? CargoName { get; set; }
        public string? Charterer { get; set; }
        public string? Remark { get; set; }
        public string? Berth { get; set; }
        
        [ForeignKey("VesselDataRawID")]
        [InverseProperty("TbPortActivitiesData")]
        public virtual TbVesselRawDatum VesselDataRaw { get; set; } = null!;
    }
}
