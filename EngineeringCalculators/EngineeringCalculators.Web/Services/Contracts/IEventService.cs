namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IEventService
    {
        event Action? IndexedDbRestored;

        void OnIndexedDbRestored();
    }
}