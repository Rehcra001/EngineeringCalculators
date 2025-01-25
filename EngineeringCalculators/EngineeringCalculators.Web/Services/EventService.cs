using EngineeringCalculators.Web.Services.Contracts;

namespace EngineeringCalculators.Web.Services
{
    public class EventService : IEventService
    {
        public event Action? IndexedDbRestored;

        public void OnIndexedDbRestored()
        {
            IndexedDbRestored?.Invoke();
        }
    }
}
