using Contracts.EntityContracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityRepositories
{
    public class PortActivitiesRepository : RepositoryBase<TbPortActivities>, IPortActivitiesRepository
    {
        public PortActivitiesRepository(ShipWatchDataContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
