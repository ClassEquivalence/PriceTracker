
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class ShopRepository : EFGenericDomainRepository<ShopDto, ShopEntity,
        PriceTrackerContext>
    {
        public ShopRepository(EFGenericEntityRepository<ShopEntity, PriceTrackerContext> entityRepository,
            ICoreToEntityMapper<ShopDto, ShopEntity> mapper) : base(entityRepository, mapper)
        {

        }
    }
}
