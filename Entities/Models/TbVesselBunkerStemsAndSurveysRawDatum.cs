using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("tb_Vessel_BunkerStemsAndSurveysRawData")]
    public partial class TbVesselBunkerStemsAndSurveysRawDatum
    {
        [Key]
        public long Id { get; set; }
        public long VesselRawDataId { get; set; }
        public int? Number { get; set; }
        [StringLength(512)]
        public string? Grade { get; set; }
        public string? StemOrSurvey { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BdnFigure { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ShipsReceivedFigure { get; set; }
        public string? SurveyRobDifferential { get; set; }

        [Column(TypeName = "decimal(27, 20)")]
        public decimal? DebunkeredFigure { get; set; }
        [ForeignKey("VesselRawDataId")]
        [InverseProperty("TbVesselBunkerStemsAndSurveysRawData")]
        public virtual TbVesselRawDatum VesselRawData { get; set; } = null!;
    }
}
