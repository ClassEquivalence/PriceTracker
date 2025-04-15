using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Models.DataAccess.Repositories
{

    public class MerchRepository: EFGenericRepository<MerchModel, MerchEntity>
    {
        private readonly IDomainToEntityMapper<MerchModel, MerchEntity> _mappingContext;

        public MerchRepository(PriceTrackerContext dbContext, EntityToModelMappingContext mappingContext) : base(dbContext)
        {
            _mappingContext = mappingContext;
        }


        protected override MerchModel EntityToModel(MerchEntity entity) => _mappingContext.Map(entity);
        protected override MerchEntity ModelToEntity(MerchModel model) => _mappingContext.Map(model);
    }
}
