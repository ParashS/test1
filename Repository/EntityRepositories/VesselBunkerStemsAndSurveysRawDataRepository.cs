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
    public class VesselBunkerStemsAndSurveysRawDataRepository : RepositoryBase<TbVesselBunkerStemsAndSurveysRawDatum>, IVesselBunkerStemsAndSurveysRawDataRepository
    {
        public VesselBunkerStemsAndSurveysRawDataRepository(ShipWatchDataContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
