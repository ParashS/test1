namespace ServiceContracts.EntityContracts
{
    public interface IVesselMetaDataService
    {
        Task<List<string?>> GetActiveImoNumbersAsync();
    }
}
