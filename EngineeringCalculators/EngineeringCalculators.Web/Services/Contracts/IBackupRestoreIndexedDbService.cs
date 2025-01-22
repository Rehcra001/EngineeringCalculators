
namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IBackupRestoreIndexedDbService
    {
        Task BackupDatabaseAsync();
    }
}