using CloudNimble.BlazorEssentials.IndexedDb;
using EngineeringCalculators.Web.Data;
using EngineeringCalculators.Web.Services.Contracts;

namespace EngineeringCalculators.Web.Services
{
    public class IndexedDbService : IIndexedDbService
    {
        private readonly EngineeringCalculatorsDb _engCalcDb;
        private readonly List<IndexedDbObjectStore> _indexedDbObjectStores;

        public IndexedDbService(EngineeringCalculatorsDb engCalcDb)
        {
            _engCalcDb = engCalcDb;

            _indexedDbObjectStores = _engCalcDb.ObjectStores;
        }

        public IndexedDbObjectStore? GetStore(string storeName)
        {
            IndexedDbObjectStore? store = null;

            if (String.IsNullOrWhiteSpace(storeName) == false)
            {
                store = _indexedDbObjectStores.Find(x => x.Name.Equals(storeName));
            }
            return store;
        }

        public async Task AddAsync<T>(T data, string storeName) where T : class
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                await store.AddAsync<T>(data);
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task BatchAddAsync<T>(T[] data, string storeName) where T : class
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                await store.BatchAddAsync<T>(data);
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task UpdateAsync<T>(T data, string storeName) where T : class
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                await store.PutAsync<T>(data);
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task DeleteAsync(int id, string storeName)
        {
            int[] ids = [id];
            IndexedDbObjectStore? store = GetStore(storeName);


            if (store is not null)
            {
                await store.BatchDeleteAsync(ids);
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task<List<T>> GetAllAsync<T>(string storeName) where T : class
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                return await store.GetAllAsync<T>();
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task<T?> GetAsync<T>(int id, string storeName) where T : class
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                return await store.GetAsync<int, T>(id);
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        public async Task ClearStoreDataAsync(string storeName)
        {
            IndexedDbObjectStore? store = GetStore(storeName);

            if (store is not null)
            {
                 await store.ClearStoreAsync();
            }
            else
            {
                throw new Exception("Store Name Invalid");
            }
        }

        
    }

}
