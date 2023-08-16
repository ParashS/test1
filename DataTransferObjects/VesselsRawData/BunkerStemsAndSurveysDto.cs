using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class BunkerStemsAndSurveysDto
    {
        public int? Number { get; set; }
        [StringLength(512)]
        public string? Grade { get; set; }
        public string? StemOrSurvey { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? BdnFigure { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ShipsReceivedFigure { get; set; }
        public string? SurveyRobDifferential { get; set; }
    }
}
