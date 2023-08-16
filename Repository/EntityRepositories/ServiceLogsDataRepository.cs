using Contracts.EntityContracts;
using Entities;
using Entities.Models;

namespace Repository.EntityRepositories
{
  public class ServiceLogsDataRepository : RepositoryBase<TbServiceLogs>, IServiceLogsDataRepository
  {
    public ServiceLogsDataRepository(ShipWatchDataContext repositoryContext)
        : base(repositoryContext)
    {
    }
  }
}
