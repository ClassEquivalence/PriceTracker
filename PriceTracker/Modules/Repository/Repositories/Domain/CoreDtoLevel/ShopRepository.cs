
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel
{
    public class ShopRepository : EFGenericDomainRepository<ShopDto, ShopEntity>
    {
        public ShopRepository(EFGenericEntityRepository<ShopEntity> entityRepository,
            ICoreToEntityMapper<ShopDto, ShopEntity> mapper) : base(entityRepository, mapper)
        {

        }
    }
}
