using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IRepositoryFacade : ICitilinkMerchRepositoryFacade,
        IShopSelectorFacade, IMerchRepositoryFacade, IShopRepositoryFacade,
        IPriceHistoryRepositoryFacade, ITimestampedPriceRepositoryFacade,
        ICitilinkMiscellaneousRepositoryFacade
    {

        (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened();

        void SetStartTimeExtractionProcessHappened(DateTime time);
        void SetFinishTimeExtractionProcessHappened(DateTime time);

        void EnsureRepositoryInitialized();
    }
}
