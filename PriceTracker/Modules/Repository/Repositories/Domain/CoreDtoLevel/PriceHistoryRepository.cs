using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class PriceHistoryRepository : EFGenericDomainRepository<MerchPriceHistoryDto,
        MerchPriceHistoryEntity>
    {
        public PriceHistoryRepository(EFGenericEntityRepository<MerchPriceHistoryEntity> entityRepository, 
            ICoreToEntityMapper<MerchPriceHistoryDto, MerchPriceHistoryEntity> mapper) : base(entityRepository, mapper)
        {
        }
    }
}
