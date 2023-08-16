using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
  [Table("tb_ServiceNotifications")]
  public class TbServiceNotifications
  {
    [Key]
    public long ServiceNotificationID { get; set; }

    public string? CustomerName { get; set; }

    [MaxLength(50)]
    public string? VesselName { get; set; }

    [MaxLength(30)]
    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    public string? EmailSubject { get; set; }

    [MaxLength(300)]
    public string? EmailReceivedFrom { get; set; }

    public string? FileName { get; set; }

    public DateTime EmailTime { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    [MaxLength(500)]
    public string? ResourceUri { get; set; }
  }
}
