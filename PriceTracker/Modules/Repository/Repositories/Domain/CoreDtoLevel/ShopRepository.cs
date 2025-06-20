using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Repositories.Base;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Core.Models.Domain;

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
