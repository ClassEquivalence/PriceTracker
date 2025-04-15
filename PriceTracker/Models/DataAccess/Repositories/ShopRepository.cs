using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DataAccess.Mapping;

namespace PriceTracker.Models.DataAccess.Repositories
{
    public class ShopRepository: EFGenericRepository<ShopModel, ShopEntity>
    {

        private readonly IDomainToEntityMapper<ShopModel, ShopEntity> _mappingContext;

        public ShopRepository(PriceTrackerContext context, EntityToModelMappingContext mappingContext) : base(context)
        {
            _mappingContext = mappingContext;
        }


        protected override ShopModel EntityToModel(ShopEntity entity) => _mappingContext.Map(entity);
        protected override ShopEntity ModelToEntity(ShopModel model) => _mappingContext.Map(model);

    }
}
