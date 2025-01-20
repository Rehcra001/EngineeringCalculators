using KristofferStrube.Blazor.FileSystemAccess;

namespace EngineeringCalculators.Web.Services
{
    public class BackupRestoreIndexedDbService
    {
        private IFileSystemAccessService _fileSystemAccessService;

        public BackupRestoreIndexedDbService(IFileSystemAccessService fileSystemAccessService)
        {
            _fileSystemAccessService = fileSystemAccessService;
        }
    }
}
