using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.DataAccess.Mapping;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories
{
    public class ShopRepository : EFGenericRepository<ShopModel, ShopEntity>
    {

        private readonly IBidirectionalDomainEntityMapper<ShopModel, ShopEntity> _mappingContext;

        protected override List<ShopEntity> listOfEntities
        {
            get
            {
                return entities.Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
                    .Include(s => s.Merches).ToList();
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
