using Contracts;
using DataTransferObjects.MailSettings;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using ServiceContracts;
using ServiceContracts.EntityContracts;
using Services.EntityServices;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IVesselRawDataService> _lazyVesselRawDataService;
        private readonly Lazy<IServiceLogsDataService> _lazyServiceLogsDataService;
        private readonly Lazy<IVesselMetaDataService> _lazyVesselMetaDataService;
        private readonly IOptions<MailSettings> _mailSettingsOptions;
        private readonly ISendGridClient _sendGridClient;
        private readonly ShipWatchDataContext _dataContext;

        public ServiceManager(IRepositoryWrapper repositoryManager, IOptions<MailSettings> mailSettingsOptions, ISendGridClient sendGrid, ShipWatchDataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _lazyVesselRawDataService = new Lazy<IVesselRawDataService>(() => new VesselRawDataService(repositoryManager, _dataContext));
            _mailSettingsOptions = mailSettingsOptions;
            _sendGridClient = sendGrid;
            _lazyServiceLogsDataService = new Lazy<IServiceLogsDataService>(() => new ServiceLogsDataService(repositoryManager, _mailSettingsOptions, _sendGridClient));
            _lazyVesselMetaDataService = new Lazy<IVesselMetaDataService>(() => new VesselMetaDataService(repositoryManager));
        }

        public IVesselRawDataService VesselRawDataService => _lazyVesselRawDataService.Value;
        public IServiceLogsDataService ServiceLogsService => _lazyServiceLogsDataService.Value;
        public IVesselMetaDataService VesselMetaDataService => _lazyVesselMetaDataService.Value;
    }
}
