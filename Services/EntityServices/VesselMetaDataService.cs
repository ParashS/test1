using Contracts;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.EntityContracts;

namespace Services.EntityServices
{
    public class VesselMetaDataService : IVesselMetaDataService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public VesselMetaDataService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        /// <summary>Gets the active imo numbers asynchronous.</summary>
        /// <returns>
        ///   List of the active vessels' imo numbers.
        /// </returns>
        public async Task<List<string?>> GetActiveImoNumbersAsync()
        {
            var activeVesselsList = await _repositoryWrapper.VesselMetaData.GetByCondition(c => c.IsActive == true).ToListAsync();

            if (activeVesselsList is not null)
            {
                return activeVesselsList.Select(c => Convert.ToString(c.ImoNumber)).ToList();
            }

            return new List<string?>();
        }
    }
}
