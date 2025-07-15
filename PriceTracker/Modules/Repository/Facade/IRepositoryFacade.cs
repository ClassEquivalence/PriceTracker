using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.Repository.Facade
{
    public interface IRepositoryFacade : ICitilinkMerchRepositoryFacade,
        IShopSelectorFacade, IExtractionExecutionStateProvider<CitilinkExtractionStateDto>,
        IMerchRepositoryFacade, IShopRepositoryFacade, IPriceHistoryRepositoryFacade,
        ITimestampedPriceRepositoryFacade, ICitilinkMiscellaneousRepositoryFacade
    {

        (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened();

        void SetStartTimeExtractionProcessHappened(DateTime time);
        void SetFinishTimeExtractionProcessHappened(DateTime time);

        void EnsureRepositoryInitialized();
    }
}
