using CloudNimble.BlazorEssentials.IndexedDb;

namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IIndexedDbService
    {
        Task AddAsync<T>(T data, string storeName) where T : class;
        Task BatchAddAsync<T>(T[] data, string storeName) where T : class;
        Task DeleteAsync(int id, string storeName);
        Task<List<T>> GetAllAsync<T>(string storeName) where T : class;
        Task<T?> GetAsync<T>(int id, string storeName) where T : class;
        Task UpdateAsync<T>(T data, string storeName) where T : class;
        Task ClearStoreDataAsync(string storeName);
        IndexedDbObjectStore? GetStore(string storeName);
    }
}