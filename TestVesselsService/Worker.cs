using DataTransferObjects.VesselsRawData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using ServiceContracts;
using Services;
using System.Collections;
using System.Text;
using System.Xml;
using TestVesselsService.ApiModels;

namespace TestVesselsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await System.Threading.Tasks.Task.Delay(1000, stoppingToken);
            }
        }
    }
}