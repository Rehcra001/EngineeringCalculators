using CloudNimble.BlazorEssentials.IndexedDb;
using EngineeringCalculators.Web.Constants;
using EngineeringCalculators.Web.Data;
using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.JSInterop;
using System.Text.Json;

namespace EngineeringCalculators.Web.Services
{
    public class BackupIndexedDbService : IBackupIndexedDbService
    {
        private readonly IFileSystemAccessService _fileSystemAccessService;
        private readonly EngineeringCalculatorsDb _EngCalcDb;
        private readonly IJSRuntime _jsRuntime;
        private List<IndexedDbObjectStore> _engCalcDBObjectStores = [];
        private FileSystemDirectoryHandle? _backupDirectoryHandle;
        private FileSystemFileHandle? _backupDataFileHandle;
        private PermissionState _readWritePermissionState;
        private bool _folderSelected = false;

        public BackupIndexedDbService(IFileSystemAccessService fileSystemAccessService,
                                             EngineeringCalculatorsDb indexedDb,
                                             IJSRuntime jSRuntime)
        {
            _fileSystemAccessService = fileSystemAccessService;
            _EngCalcDb = indexedDb;
            _jsRuntime = jSRuntime;
        }

        public async Task BackupDatabaseAsync()
        {
            await _EngCalcDb.OpenAsync();

            GetObjectStores();

            await GetFolderHandleAsync();

            if (_folderSelected)
            {
                await RequestReadWritePermissionAsync();

                await GenerateFileHandlesAsync();
            }
            
        }

        private void GetObjectStores()
        {
            _engCalcDBObjectStores.Clear();
            _engCalcDBObjectStores = _EngCalcDb.ObjectStores;

        }

        private async Task GetFolderHandleAsync()
        {
            try
            {
                var options = new DirectoryPickerOptionsStartInWellKnownDirectory()
                {
                    StartIn = WellKnownDirectory.Documents
                };

                var fsOptions = new FileSystemOptions();

                _backupDirectoryHandle = await _fileSystemAccessService.ShowDirectoryPickerAsync(options);
                _folderSelected = true;
            }
            catch (Exception ex)
            {
                _folderSelected = false;
                await _jsRuntime.InvokeVoidAsync("console.log", ex.Message);
            }

        }

        private async Task GenerateFileHandlesAsync()
        {
            if (_engCalcDBObjectStores.Count > 0)
            {
                foreach (IndexedDbObjectStore store in _engCalcDBObjectStores)
                {
                    await CreateHandleAsync(store);
                }
            }
        }
        private async Task CreateHandleAsync(IndexedDbObjectStore store)
        {
            FileSystemGetFileOptions fileOptions = new FileSystemGetFileOptions()
            {
                Create = true
            };

            _backupDataFileHandle = await _backupDirectoryHandle!.GetFileHandleAsync(store.Name + ".json", fileOptions);

            await GetStoreData();
        }

        private async Task GetStoreData()
        {
            foreach (IndexedDbObjectStore store in _engCalcDBObjectStores)
            {
                switch (store.Name)
                {
                    case IndexedDbStoreNameConstants.MaterialsStore:
                        List<MaterialModel> materials = await store.GetAllAsync<MaterialModel>();
                        await SaveDataAsync(materials);
                        break;
                }
            }
        }

        private async Task SaveDataAsync<T>(List<T> dataFile)
        {
            if (dataFile is not null && dataFile.Count > 0)
            {
                var options = new JsonSerializerOptions();
                options.WriteIndented = true;
                string jsonMaterialsFile = JsonSerializer.Serialize<List<T>>(dataFile, options);

                if (_backupDataFileHandle is not null)
                {
                    var writable = await _backupDataFileHandle.CreateWritableAsync();
                    await writable.WriteAsync(jsonMaterialsFile);
                    await writable.CloseAsync();
                }
            }
            
        }


        private async Task RequestReadWritePermissionAsync()
        {
            if (_backupDirectoryHandle is not null)
            {
                _readWritePermissionState = await _backupDirectoryHandle.QueryPermissionAsync(new() { Mode = FileSystemPermissionMode.ReadWrite });

                if (_readWritePermissionState != 0)
                {
                    _readWritePermissionState = await _backupDirectoryHandle.RequestPermissionAsync(new() { Mode = FileSystemPermissionMode.ReadWrite });
                }
            }
        }


    }
}
