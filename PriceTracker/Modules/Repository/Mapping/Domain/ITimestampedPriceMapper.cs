using PriceTracker.Core.CommonModels.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public interface ITimestampedPriceMapper : ICoreToEntityMapper<TimestampedPriceDto,
        TimestampedPriceEntity>
    {
    }
}
