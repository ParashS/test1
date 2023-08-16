using DataTransferObjects.ReportForm;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using Serilog;
using ServiceContracts;
using System.Net;
using System.Net.Http.Headers;
using Vessels.ApiModels;

namespace Vessels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawReportsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;

        public RawReportsController(IServiceManager serviceManager, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _configuration = configuration;
        }

        /// <summary>
        /// This Apis is used for enter the Vessel raw data from the website for different reoprt type
        /// </summary>
        /// <param name="reportForm"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ManualEntry")]
        public async Task<IActionResult> SubmitManualRawReport(ReportFormDto reportForm)
        {
            if (reportForm != null && ModelState.IsValid)
            {
                var response = await _serviceManager.VesselRawDataService.AddManualRawReportAsync(reportForm);
                if (response > 0)
                {
                    var NdrApiUrl = _configuration["API:NdrApiUrl"] + response;
                    var NdrClient = new RestClient(NdrApiUrl);
                    var NdrRequest = new RestRequest(NdrApiUrl, RestSharp.Method.Post);
                    var responseData = await NdrClient.ExecutePostAsync(NdrRequest);

                    await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
                    {
                        ServiceName = "NDR Request response",
                        StartedDateTime = DateTime.UtcNow,
                        EndedDateTime = DateTime.UtcNow,
                        ResourceUri = new Uri("/ManualEntry"),
                        RequestJSONBody = "",
                        ResponseJSONBody = JsonConvert.SerializeObject(responseData),
                        RecordsToBeProcessed = 0,
                        RecordsActuallyProcessed = 0,
                        Content = null,
                        StatusCode = "",
                        StatusDescription = "",
                        Source = "",
                        Message = "",
                        InnerException = "",
                        StackTrace = ""
                    });

                    return Ok(new CommonApiResponse()
                    {
                        statusCode = (int)HttpStatusCode.OK,
                        message = "Data saved successfully",
                        data = new()
                    });
                }

                return BadRequest(new CommonApiResponse()
                {
                    statusCode = (int)HttpStatusCode.BadRequest,
                    message = "Something went wrong !",
                    data = new()
                });
            }

            List<string> errorMessages = new List<string>();
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    errorMessages.Add(error.ErrorMessage);
                }
            }
            return BadRequest(new CommonApiResponse()
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = "Invalid Data",
                data = errorMessages
            });
        }

        [HttpPost]
        [Route("CallNdrApiForManualRawReport")]
        public async Task<IActionResult> CallNdrApiForManualRawReport(long vesselRawDataId)
        {
            var NdrApiUrl = _configuration["API:NdrApiUrl"] + vesselRawDataId;
            var NdrClient = new RestClient(NdrApiUrl);
            var NdrRequest = new RestRequest(NdrApiUrl, Method.Post);
            var responseData = await NdrClient.ExecutePostAsync(NdrRequest);

            await _serviceManager.ServiceLogsService.CreateAsync(new TbServiceLogs()
            {
                ServiceName = "NDR Request response",
                StartedDateTime = DateTime.UtcNow,
                EndedDateTime = DateTime.UtcNow,
                ResourceUri = new Uri("http://202.131.117.91:81/SwVessels/Api/ManualEntry"),
                RequestJSONBody = "",
                ResponseJSONBody = JsonConvert.SerializeObject(responseData),
                RecordsToBeProcessed = 0,
                RecordsActuallyProcessed = 0,
                Content = null,
                StatusCode = "",
                StatusDescription = "",
                Source = "",
                Message = "",
                InnerException = "",
                StackTrace = ""
            });

            return Ok(new CommonApiResponse()
            {
                statusCode = (int)HttpStatusCode.OK,
                message = "Success",
                data = null
            });
        }

    }
}
