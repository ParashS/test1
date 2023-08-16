using Contracts.EntityContracts;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IVesselRawDataRepository VesselRawData { get; }
        IVesselForobRawDataRepository VesselForobRawData { get; }
        IVesselForobAllocationRawDataRepository VesselForobAllocationRawData { get; }
        IVesselBunkerTankRawDataRepository VesselBunkerTankRawData { get; }
        IVesselsAuxilliaryRawDataRepository VesselsAuxilliaryRawData { get; }
        IVesselUpcomingPortRawDataRepository VesselUpcomingPortRawData { get; }
        IVesselBunkerStemsAndSurveysRawDataRepository VesselBunkerStemsAndSurveysRawData { get; }
        ICargoHandlingRepository CargoHandlingData { get; }
        IStowageRepository StowageData { get; }
        IPortActivitiesRepository PortActivities { get; }
        IServiceLogsDataRepository ServiceLogsData { get; }
        IServiceNotificationsDataRepository ServiceNotificationsData { get; }
        IVesselMetaDataRepository VesselMetaData { get; }

        void Save();
        Task SaveAync();
    }
}
