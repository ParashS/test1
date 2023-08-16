using Contracts.EntityContracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityRepositories
{
    public class VesselForobAllocationRawDataRepository : RepositoryBase<TbVesselForobAllocationRawDatum>, IVesselForobAllocationRawDataRepository
    {
        public VesselForobAllocationRawDataRepository(ShipWatchDataContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
