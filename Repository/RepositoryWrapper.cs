using Contracts;
using Contracts.EntityContracts;
using Entities;
using Repository.EntityRepositories;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly ShipWatchDataContext _repoContext;

        public RepositoryWrapper(ShipWatchDataContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        private IVesselRawDataRepository? _vesselRawData;

        public IVesselRawDataRepository VesselRawData
        {
            get
            {
                _vesselRawData ??= new VesselRawDataRepository(_repoContext);
                return _vesselRawData;
            }
        }

        private IVesselForobRawDataRepository? _vesselForobRawData;
        public IVesselForobRawDataRepository VesselForobRawData
        {
            get
            {
                _vesselForobRawData ??= new VesselForobRawDataRepository(_repoContext);
                return _vesselForobRawData;
            }
        }

        private IVesselForobAllocationRawDataRepository? _vesselForobAllocationRawData;
        public IVesselForobAllocationRawDataRepository VesselForobAllocationRawData
        {
            get
            {
                _vesselForobAllocationRawData ??= new VesselForobAllocationRawDataRepository(_repoContext);
                return _vesselForobAllocationRawData;
            }
        }

        private IVesselBunkerTankRawDataRepository? _vesselBunkerTankRawData;
        public IVesselBunkerTankRawDataRepository VesselBunkerTankRawData
        {
            get
            {
                _vesselBunkerTankRawData ??= new VesselBunkerTankRawDataRepository(_repoContext);
                return _vesselBunkerTankRawData;
            }
        }

        private IVesselsAuxilliaryRawDataRepository? _vesselsAuxilliaryRawData;
        public IVesselsAuxilliaryRawDataRepository VesselsAuxilliaryRawData
        {
            get
            {
                _vesselsAuxilliaryRawData ??= new VesselsAuxilliaryRawDataRepository(_repoContext);
                return _vesselsAuxilliaryRawData;
            }
        }

        private IVesselUpcomingPortRawDataRepository? _vesselUpcomingPortRawData;
        public IVesselUpcomingPortRawDataRepository VesselUpcomingPortRawData
        {
            get
            {
                _vesselUpcomingPortRawData ??= new VesselUpcomingPortRawDataRepository(_repoContext);
                return _vesselUpcomingPortRawData;
            }
        }

        private IVesselBunkerStemsAndSurveysRawDataRepository? _vesselBunkerStemsAndSurveysRawData;
        public IVesselBunkerStemsAndSurveysRawDataRepository VesselBunkerStemsAndSurveysRawData
        {
            get
            {
                _vesselBunkerStemsAndSurveysRawData ??= new VesselBunkerStemsAndSurveysRawDataRepository(_repoContext);
                return _vesselBunkerStemsAndSurveysRawData;
            }
        }

        private ICargoHandlingRepository? _cargoHandlingRepository;
        public ICargoHandlingRepository CargoHandlingData
        {
            get
            {
                _cargoHandlingRepository ??= new CargoHandlingRepository(_repoContext);
                return _cargoHandlingRepository;
            }
        }

        private IStowageRepository? _stowageRepository;
        public IStowageRepository StowageData
        {
            get
            {
                _stowageRepository ??= new StowagesRepositiory(_repoContext);
                return _stowageRepository;
            }
        }

        private IPortActivitiesRepository? _portActivities;
        public IPortActivitiesRepository PortActivities
        {
            get
            {
                _portActivities ??= new PortActivitiesRepository(_repoContext);
                return _portActivities;
            }
        }

        private IServiceLogsDataRepository? _serviceLogsData;
        public IServiceLogsDataRepository ServiceLogsData
        {
            get
            {
                _serviceLogsData ??= new ServiceLogsDataRepository(_repoContext);
                return _serviceLogsData;
            }
        }

        private IServiceNotificationsDataRepository? _serviceNotificationsData;
        public IServiceNotificationsDataRepository ServiceNotificationsData
        {
            get
            {
                _serviceNotificationsData ??= new ServiceNotificationsDataRepository(_repoContext);
                return _serviceNotificationsData;
            }
        }

        private IVesselMetaDataRepository? _vesselMetaDataRepository;
        public IVesselMetaDataRepository VesselMetaData
        {
            get
            {
                _vesselMetaDataRepository ??= new VesselMetaDataRepository(_repoContext);
                return _vesselMetaDataRepository;
            }
        }

        public IVesselMetaDataRepository vesselMetaDataRepository => throw new NotImplementedException();

        public void Save()
        {
            _repoContext.SaveChanges();
        }

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
