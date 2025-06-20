using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class TimestampedPriceRepository : EFGenericDomainRepository<TimestampedPriceDto, TimestampedPriceEntity>
    {
        public TimestampedPriceRepository(EFGenericEntityRepository<TimestampedPriceEntity> entityRepository, 
            ICoreToEntityMapper<TimestampedPriceDto, TimestampedPriceEntity> mapper) : base(entityRepository, mapper)
        {
        }
    }
}
