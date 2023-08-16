using Contracts.EntityContracts;
using Entities;
using Entities.Models;

namespace Repository.EntityRepositories
{
  public class ServiceNotificationsDataRepository : RepositoryBase<TbServiceNotifications>, IServiceNotificationsDataRepository
  {
    public ServiceNotificationsDataRepository(ShipWatchDataContext repositoryContext)
        : base(repositoryContext)
    {
    }
  }
}
