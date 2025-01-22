using CloudNimble.BlazorEssentials.IndexedDb;
using Microsoft.JSInterop;

namespace EngineeringCalculators.Web.Data
{
    public class EngineeringCalculatorsDb : IndexedDbDatabase
    {
        [ObjectStore(AutoIncrementKeys = false)]
        [Index(Name = "Id", Path = "id")]
        [Index(Name = "Name", Path = "name")]
        public IndexedDbObjectStore Materials { get; set; }

        public EngineeringCalculatorsDb(IJSRuntime jSRuntime): base(jSRuntime)
        {            
            Version = 1;
        }
    }

}
