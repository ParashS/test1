using ServiceContracts.EntityContracts;

namespace ServiceContracts
{
    public interface IServiceManager
    {
        IVesselRawDataService VesselRawDataService { get; }
        IServiceLogsDataService ServiceLogsService { get; }
        IVesselMetaDataService VesselMetaDataService { get; }
    }
}
