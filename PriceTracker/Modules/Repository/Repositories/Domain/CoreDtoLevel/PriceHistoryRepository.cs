using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class PriceHistoryRepository : EFGenericDomainRepository<MerchPriceHistoryDto,
        MerchPriceHistoryEntity, PriceTrackerContext>
    {
        public PriceHistoryRepository(EFGenericEntityRepository<MerchPriceHistoryEntity, PriceTrackerContext> entityRepository,
            ICoreToEntityMapper<MerchPriceHistoryDto, MerchPriceHistoryEntity> mapper) : base(entityRepository, mapper)
        {
        }
    }
}
