using CloudNimble.BlazorEssentials.IndexedDb;
using EngineeringCalculators.Web.Data;
using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Pages;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EngineeringCalculators.Web.Services
{
    public class BackupRestoreIndexedDbService : IBackupRestoreIndexedDbService
    {
        private readonly IFileSystemAccessService _fileSystemAccessService;
        private readonly EngineeringCalculatorsDb _EngClacDb;
        private readonly FolderAndFileHandlesDb _handlesDb;
        private List<IndexedDbObjectStore> _engCalcDBObjectStores = [];
        private List<IndexedDbObjectStore> _handlesDbObjectStores = [];
        private FolderHandleModel? _backupDirectoryHandle;
        private List<FileHandleModel>? _backupFileHandles;
        private PermissionState _readWritePermissionState;
        private string _directoryHandleName;
        public BackupRestoreIndexedDbService(IFileSystemAccessService fileSystemAccessService,
                                             EngineeringCalculatorsDb indexedDb,
                                             FolderAndFileHandlesDb handlesDb)
        {
            _fileSystemAccessService = fileSystemAccessService;
            _EngClacDb = indexedDb;
            _handlesDb = handlesDb;
        }

        public async Task BackupDatabaseAsync()
        {
            await _EngClacDb.OpenAsync();

            await _handlesDb.OpenAsync();

            GetObjectStores();

            await GetFolderHandleAsync();

            await RequestReadWritePermissionAsync();

            await GetFileHandlesAsync();

            

            foreach (IndexedDbObjectStore store in _engCalcDBObjectStores)
            {
                switch (store.Name)
                {
                    case "Materials":
                        List<MaterialModel> materials = await store.GetAllAsync<MaterialModel>();
                        await SaveDataAsync(materials);
                        break;
                }
            }
        }        

        private void GetObjectStores()
        {
            _engCalcDBObjectStores.Clear();
            _engCalcDBObjectStores = _EngClacDb.ObjectStores;

            _handlesDbObjectStores.Clear();
            _handlesDbObjectStores = _handlesDb.ObjectStores;
        }

        private async Task GetFolderHandleAsync()
        {
            if (await _handlesDb.FolderHandles.CountAsync() > 0)
            {
                await RetrieveBackupFolderHandle();
            }
            else
            {
                try
                {
                    var options = new DirectoryPickerOptionsStartInWellKnownDirectory()
                    {
                        StartIn = WellKnownDirectory.Documents
                    };

                    var fsOptions = new FileSystemOptions();

                    var handle = await _fileSystemAccessService.ShowDirectoryPickerAsync(options);


                    _backupDirectoryHandle = new FolderHandleModel()
                    {
                        Id = 1,
                        Name = "Backup",
                        FolderHandle = handle
                    };

                    await SaveFolderHandleAsync();


                }
                catch (Exception)
                {

                    throw;
                }
            }
            
        }

        private async Task RetrieveBackupFolderHandle()
        {
            try
            {
                _backupDirectoryHandle = await _handlesDb.FolderHandles.GetAsync<int, FolderHandleModel>(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private async Task SaveFolderHandleAsync()
        {
            try
            {
                //    var options = new JsonSerializerSettings();
                //    options.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                //    var jstring = JsonConvert.SerializeObject(_backupDirectoryHandle.FolderHandle, options);

                //    var h = JsonConvert.DeserializeObject<FileSystemDirectoryHandle>(jstring);

                await _handlesDb.FolderHandles.AddAsync<FolderHandleModel>(_backupDirectoryHandle);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task GetFileHandlesAsync()
        {
            _backupFileHandles = await _handlesDb.FileHandles.GetAllAsync<FileHandleModel>();

            if (_engCalcDBObjectStores.Count > 0)
            {
                foreach (IndexedDbObjectStore store in _engCalcDBObjectStores)
                {
                    bool hasHandle = _backupFileHandles.Any(x => x.Name.Equals(store.Name));
                    if (hasHandle == false)
                    {
                        await CreateHandleAsync(store);
                    }
                }
            }
        }
        private async Task CreateHandleAsync(IndexedDbObjectStore store)
        {
            int id = 1;
            string storeName = store.Name;
            
            if (_backupFileHandles is not null && _backupFileHandles.Count > 0)
            {
                id = _backupFileHandles.Max(x => x.Id);
            }

            FileSystemGetFileOptions fileOptions = new FileSystemGetFileOptions()
            {
                Create = true
            };          

            FileHandleModel handle = new FileHandleModel()
            {
                Id = id,
                Name = storeName,
                FileHandle = await _backupDirectoryHandle.FolderHandle.GetFileHandleAsync(storeName + ".json", fileOptions)
            };

            await SaveFileHandleAsync(handle);
        }

        private async Task SaveFileHandleAsync(FileHandleModel handle)
        {
            await _handlesDb.FileHandles.AddAsync<FileHandleModel>(handle);
        }

        public async Task SaveDataAsync<T>(List<T> dataFile)
        {
            var options = new JsonSerializerSettings();
            options.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            string jsonMaterialsFile = JsonConvert.SerializeObject(dataFile, options);
        }


        private async Task RequestReadWritePermissionAsync()
        {
            if (_backupDirectoryHandle is not null)
            {
                _directoryHandleName = await _backupDirectoryHandle.FolderHandle.GetNameAsync();
                _readWritePermissionState = await _backupDirectoryHandle.FolderHandle.QueryPermissionAsync(new() { Mode = FileSystemPermissionMode.ReadWrite });

                if (_readWritePermissionState != 0)
                {
                    _readWritePermissionState = await _backupDirectoryHandle.FolderHandle.RequestPermissionAsync(new() { Mode = FileSystemPermissionMode.ReadWrite });
                }
            }
        }

        
    }
}
