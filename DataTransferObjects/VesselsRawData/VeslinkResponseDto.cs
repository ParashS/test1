using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
  public class VeslinkResponseDto
  {
    public string? formID { get; set; }
    public string? success { get; set; }
    public string? formStatus { get; set; }
    public string[]? formErrors { get; set; }
    public string? message { get; set; }
  }
}
