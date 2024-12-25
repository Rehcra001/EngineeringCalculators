using EngineeringCalculators.Web.Models;
using KristofferStrube.Blazor.FileSystem;

namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IMaterialService
    {
        FileSystemFileHandle? FileHandle { get; }
        Task<List<MaterialModel>> GetAllAsync();
        Task SaveAllAsync(List<MaterialModel> materials);
        Task UpdateAsync(List<MaterialModel> materials);
    }
}