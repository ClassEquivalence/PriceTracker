using PriceTracker.Core.CommonModels.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public interface IPriceHistoryMapper : ICoreToEntityMapper<MerchPriceHistoryDto,
        MerchPriceHistoryEntity>
    {

    }
}
