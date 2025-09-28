using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class TimestampedPriceRepository : EFGenericDomainRepository<TimestampedPriceDto, TimestampedPriceEntity
        , PriceTrackerContext>
    {
        public TimestampedPriceRepository(IEntityRepository<TimestampedPriceEntity> entityRepository,
            ITimestampedPriceMapper mapper) : base(entityRepository, mapper)
        {
        }
    }
}
