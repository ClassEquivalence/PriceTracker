using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.Repository.Facade
{
    public interface IRepositoryFacade : ICitilinkMerchRepositoryFacade,
        IShopSelectorFacade, IExtractionExecutionStateProvider<CitilinkParsingExecutionState>,
        IMerchRepositoryFacade, IShopRepositoryFacade, IPriceHistoryRepositoryFacade,
        ITimestampedPriceRepositoryFacade, ICitilinkMiscellaneousRepositoryFacade
    {

        (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened();

        void SetStartTimeExtractionProcessHappened(DateTime time);
        void SetFinishTimeExtractionProcessHappened(DateTime time);
    }
}
