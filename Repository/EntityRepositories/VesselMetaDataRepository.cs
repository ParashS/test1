using Contracts.EntityContracts;
using Entities;
using Entities.Models;

namespace Repository.EntityRepositories
{
    public class VesselMetaDataRepository : RepositoryBase<TbVesselMetaDatum>, IVesselMetaDataRepository
    {
        public VesselMetaDataRepository(ShipWatchDataContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
