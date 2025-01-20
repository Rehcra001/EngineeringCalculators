using EngineeringCalculators.Web.Data;
using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;

namespace EngineeringCalculators.Web.Services
{
    public class MaterialndexedDbService : IMaterialndexedDbService
    {
        private readonly EngineeringCalculatorsDb IndexedDb;

        public MaterialndexedDbService(EngineeringCalculatorsDb indexedDb)
        {
            IndexedDb = indexedDb;
        }

        public async Task AddAsync(MaterialModel material)
        {
            await IndexedDb.Materials.AddAsync(material);
        }

        public async Task UpdateAsync(MaterialModel material)
        {
            await IndexedDb.Materials.PutAsync<MaterialModel>(material);
        }

        public async Task DeleteAsync(MaterialModel material)
        {
            int[] ids = [material.Id];

            await IndexedDb.Materials.BatchDeleteAsync<int>(ids);
        }

        public async Task<List<MaterialModel>> GetAllAsync()
        {
            List<MaterialModel> materials = [];

            materials = await IndexedDb.Materials.GetAllAsync<MaterialModel>();

            return materials;
        }

        public async Task<MaterialModel?> GetAsync(int id)
        {
            MaterialModel? material = new();

            material = await IndexedDb.Materials.GetAsync<int, MaterialModel>(id);

            return material;
        }
    }
}
