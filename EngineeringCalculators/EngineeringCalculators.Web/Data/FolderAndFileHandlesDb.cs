using CloudNimble.BlazorEssentials.IndexedDb;
using Microsoft.JSInterop;

namespace EngineeringCalculators.Web.Data
{
    public class FolderAndFileHandlesDb : IndexedDbDatabase
    {
        [ObjectStore(AutoIncrementKeys = true)]
        [Index(Name = "Id", Path = "id")]
        [Index(Name = "Name", Path = "name")]
        public IndexedDbObjectStore FolderHandles { get; set; }

        [ObjectStore(AutoIncrementKeys = false)]
        [Index(Name = "Id", Path = "id")]
        [Index(Name = "Name", Path = "name")]
        public IndexedDbObjectStore FileHandles { get; set; }

        public FolderAndFileHandlesDb(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            Version = 1;
        }
    }
}
