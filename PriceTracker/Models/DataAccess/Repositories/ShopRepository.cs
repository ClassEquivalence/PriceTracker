using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Entities.Domain;

namespace PriceTracker.Models.DataAccess.Repositories
{
    public class ShopRepository: EFGenericRepository<ShopModel, ShopEntity>
    {

        private readonly IBidirectionalDomainEntityMapper<ShopModel, ShopEntity> _mappingContext;

        protected override List<ShopEntity> listOfEntities
        {
            get
            {
                return entities.Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
                    .Include(s=>s.Merches).ToList();
            }
        }

        public ShopRepository(PriceTrackerContext context, BidirectionalEntityModelMappingContext mappingContext) : base(context)
        {
            _mappingContext = mappingContext;
        }


        protected override ShopModel EntityToModel(ShopEntity entity) => _mappingContext.Map(entity);
        protected override ShopEntity ModelToEntity(ShopModel model) => _mappingContext.Map(model);

    }
}
