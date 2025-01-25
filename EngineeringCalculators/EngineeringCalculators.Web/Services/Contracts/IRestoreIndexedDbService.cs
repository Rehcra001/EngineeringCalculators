namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IRestoreIndexedDbService
    {
        Task RestoreDatabaseAsync();
    }
}