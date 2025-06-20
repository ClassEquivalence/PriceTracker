using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public interface IPriceHistoryMapper : ICoreToEntityMapper<MerchPriceHistoryDto,
        MerchPriceHistoryEntity>
    {

    }
}
