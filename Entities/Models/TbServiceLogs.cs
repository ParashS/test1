using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
  [Table("tb_ServiceLogs")]
  public class TbServiceLogs
  {
    [Key]
    public long ServiceLogID { get; set; }

    public string ServiceName { get; set; } = "Veslink";

    public DateTime StartedDateTime { get; set; }

    public DateTime EndedDateTime { get; set; }

    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    public Uri? ResourceUri { get; set; }

    public string? RequestJSONBody { get; set; }

    public string? ResponseJSONBody { get; set; }

    public int RecordsToBeProcessed { get; set; }

    public int RecordsActuallyProcessed { get; set; }

    public string? Content { get; set; }

    public string? StatusCode { get; set; }

    public string? StatusDescription { get; set; }

    public string? Source { get; set; }

    public string? Message { get; set; }

    public string? InnerException { get; set; }

    public string? StackTrace { get; set; }
  }
}
