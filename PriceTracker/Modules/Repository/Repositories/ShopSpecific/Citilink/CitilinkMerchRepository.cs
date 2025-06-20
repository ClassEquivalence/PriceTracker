using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchRepository : MerchSubtypeRepository<CitilinkMerchDto,
        CitilinkMerchEntity>
    {
        public CitilinkMerchRepository(EFGenericEntityRepository<CitilinkMerchEntity> 
            entityRepository, ICoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity> 
            mapper) : base(entityRepository, mapper)
        {
        }
    }
}
