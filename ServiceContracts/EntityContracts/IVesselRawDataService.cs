using DataTransferObjects.ReportForm;
using DataTransferObjects.VesselsRawData;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.EntityContracts
{
    public interface IVesselRawDataService
    {
        Task<long> CreateAsync(VesselsRawDataDto request);
        Task<List<string>> GetAllExistedAttachmentIdAsync();
        (long?, int) GetMaxVersionNumberByReport(GetVersionNumberByReportRequestDto request);
        (List<VesselsRawDataDto>, List<TbVesselRawDatum>) GetData();

        void UpdateData(TbVesselRawDatum request);
        Task<long> AddManualRawReportAsync(ReportFormDto reportForm);
    }
}
