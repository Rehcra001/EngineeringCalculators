using EngineeringCalculators.Web.Constants;
using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.JSInterop;
using System.Text.Json;

namespace EngineeringCalculators.Web.Services
{
    public class RestoreIndexedDbService : IRestoreIndexedDbService
    {
        private readonly IFileSystemAccessService _fileSystemAccessService;
        private readonly IJSRuntime _jsRuntime;
        private FileSystemDirectoryHandle? _directoryHandle = null;
        private IFileSystemHandle[] _fileSystemHandles = [];
        private List<FileSystemFileHandle> _fileHandles = [];

        public RestoreIndexedDbService(IFileSystemAccessService fileSystemAccessService, IJSRuntime jSRuntime)
        {
            _fileSystemAccessService = fileSystemAccessService;
            _jsRuntime = jSRuntime;
        }

        public async Task RestoreDatabaseAsync()
        {
            await OpenBackupDirectoryAsync();

            if (_directoryHandle is not null)
            {
                await LoadFileSystemHandlesAsync();

                if (_fileSystemHandles.Length > 0)
                {
                    await CreateFileHandles();
                    await ExtractBackupData();
                }
            }
        }

        private async Task OpenBackupDirectoryAsync()
        {
            _directoryHandle = null;
            try
            {
                var options = new DirectoryPickerOptionsStartInWellKnownDirectory() { StartIn = WellKnownDirectory.Documents };
                _directoryHandle = await _fileSystemAccessService.ShowDirectoryPickerAsync();
            }
            catch (JSException ex)
            {
                await _jsRuntime.InvokeVoidAsync("console.log", ex.Message);
            }

        }

        private async Task LoadFileSystemHandlesAsync()
        {
            if (_directoryHandle is not null)
            {
                _fileSystemHandles = await _directoryHandle.ValuesAsync();
            }
        }

        private async Task CreateFileHandles()
        {
            _fileHandles.Clear();
            foreach (IFileSystemHandle handle in _fileSystemHandles)
            {
                var handleType = await handle.GetKindAsync();
                if (handleType is FileSystemHandleKind.File)
                {
                    _fileHandles.Add(FileSystemFileHandle.Create(_jsRuntime, handle.JSReference));
                }
            }
        }

        private async Task ExtractBackupData()
        {
            foreach (FileSystemFileHandle handle in _fileHandles)
            {
                string fileName = await handle.GetNameAsync();
                var file = await handle.GetFileAsync();
                string content = await file.TextAsync();

                if (String.IsNullOrWhiteSpace(content) == false)
                {
                    switch (fileName)
                    {
                        case BackupRestoreFilenameConstants.MaterialFile:
                            List<MaterialModel> data = [];
                            DeserializeData(ref data, content);

                            break;
                    }
                }
                
            }
        }

        private void  DeserializeData<T>(ref List<T> data, string content)
        {
            data = JsonSerializer.Deserialize<List<T>>(content)!;
        }
    }
}
