using EngineeringCalculators.Web.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace EngineeringCalculators.Web.Components
{
    public partial class BackupRestoreComponent
    {
        [Inject]
        public required IBackupIndexedDbService BackupIndexedDbService { get; set; }

        [Inject]
        public required IRestoreIndexedDbService RestoreIndexedDbService { get; set; }

        private async Task HandleBackup()
        {
            await BackupIndexedDbService.BackupDatabaseAsync();
        }

        private async Task HandleRestore()
        {
            await RestoreIndexedDbService.RestoreDatabaseAsync();
        }
    }
}
