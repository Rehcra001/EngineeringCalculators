using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using System.Text.Json;

namespace EngineeringCalculators.Web.Services
{
    public class MaterialService : IMaterialService
    {
        public FileSystemFileHandle? FileHandle { get; private set; } = null;

        private IFileSystemAccessService _fileSystemAccessService;
        private string? _fileText;        
        private string _fileHandleName = "";
        private PermissionState _readPermissionState;
        private PermissionState _writePermissionState;

        public MaterialService(IFileSystemAccessService fileSystemAccessService)
        {
            _fileSystemAccessService = fileSystemAccessService;
        }

        public async Task UpdateAsync(List<MaterialModel> materials)
        {
            if (FileHandle is null) return;

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string jsonMaterialsFile = JsonSerializer.Serialize<List<MaterialModel>>(materials, options);

            try
            {
                var file = await FileHandle.GetFileAsync();
                var writable = await FileHandle.CreateWritableAsync();
                await writable.WriteAsync(jsonMaterialsFile);
                await writable.CloseAsync();
                await ReadFileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }
        }


        public async Task SaveAllAsync(List<MaterialModel> materials)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string jsonMaterialsFile = JsonSerializer.Serialize<List<MaterialModel>>(materials, options);

            _fileHandleName = "";
            try
            {                
                FileHandle = await _fileSystemAccessService.ShowSaveFilePickerAsync();
                if (FileHandle != null)
                {
                    var file = await FileHandle.GetFileAsync();

                    _fileHandleName = await file.GetNameAsync();                    
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }
            finally
            {
                if (FileHandle is not null)
                {
                    var writable = await FileHandle.CreateWritableAsync();
                    await writable.WriteAsync(jsonMaterialsFile);
                    await writable.CloseAsync();

                    await ReadFileAsync();
                }
            }
        }

        public async Task<List<MaterialModel>> GetAllAsync()
        {
            List<MaterialModel> materials = [];
            _fileText = "";

            try
            {
                var options = new OpenFilePickerOptionsStartInWellKnownDirectory()
                {
                    Multiple = false,
                    StartIn = WellKnownDirectory.Documents
                };

                var fileHandles = await _fileSystemAccessService.ShowOpenFilePickerAsync(options);
                FileHandle = fileHandles.Single();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }
            finally
            {
                if (FileHandle != null)
                {
                    _fileHandleName = await FileHandle.GetNameAsync();
                    _readPermissionState = await FileHandle.QueryPermissionAsync(new() { Mode = FileSystemPermissionMode.Read });
                }

                await ReadFileAsync();

                if (String.IsNullOrWhiteSpace(_fileText) == false)
                {
                    materials = JsonSerializer.Deserialize<List<MaterialModel>>(_fileText)!;
                }
            }


            return materials;
        }

        private async Task ReadFileAsync()
        {
            if (FileHandle is null) return;

            var file = await FileHandle.GetFileAsync();
            _fileText = await file.TextAsync();

            _writePermissionState = await FileHandle.QueryPermissionAsync(new() { Mode = FileSystemPermissionMode.ReadWrite });
        }
    }
}
