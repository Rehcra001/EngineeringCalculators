using EngineeringCalculators.Web.Models;
using KristofferStrube.Blazor.FileSystem;

namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IMaterialFileAccessService
    {
        FileSystemFileHandle? FileHandle { get; }
        Task<List<MaterialModel>> GetAllAsync();
        Task<List<MaterialModel>> GetAllMemoryAsync();
        Task SaveAllAsync(List<MaterialModel> materials);
        Task UpdateAsync(List<MaterialModel> materials);
    }
}