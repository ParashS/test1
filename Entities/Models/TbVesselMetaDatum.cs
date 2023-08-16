using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("tb_VesselMetaData")]
    public partial class TbVesselMetaDatum
    {
        [Key]
        public long Id { get; set; }
        public long? ImoNumber { get; set; }
        [StringLength(1024)]
        public string? VesselName { get; set; }
        public DateTime? InstallationDate { get; set; }
        [StringLength(50)]
        public string? SourceType { get; set; }
        [StringLength(512)]
        public string? MasterName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }
    }
}
