using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.CommonModels.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping.Domain;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink
{
    public class CitilinkMerchCoreToEntityMapper : BaseGenericCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>,
        ICitilinkMerchCoreToEntityMapper
    {
        private readonly IPriceHistoryMapper _priceHistoryMapper;
        private int? _citilinkId;
        private readonly IDbContextFactory<PriceTrackerContext> _contextFactory;
        public CitilinkMerchCoreToEntityMapper(IDbContextFactory<PriceTrackerContext>
            contextFactory, IPriceHistoryMapper priceHistoryMapper) : base()
        {
            _priceHistoryMapper = priceHistoryMapper;
            _contextFactory = contextFactory;
        }

        protected override CitilinkMerchEntity CreateEntityFromModel(CitilinkMerchDto model)
        {
            EnsureCitilinkIdInitialized();
            CitilinkMerchEntity entity = new(model.Name, model.CitilinkId, model.Id);
            entity.ShopId = _citilinkId ?? throw new InvalidOperationException();
            entity.PriceHistory = _priceHistoryMapper.Map(model.PriceTrack);
            entity.PriceHistory.Id = model.PriceHistoryId;
            return entity;
        }

        protected override CitilinkMerchDto CreateModelFromEntity(CitilinkMerchEntity entity)
        {
            return new CitilinkMerchDto(entity.Id, entity.Name, _priceHistoryMapper.Map(entity.PriceHistory),
                entity.CitilinkId, entity.PriceHistory.Id);
        }

        protected override void MapModelFieldsToEntity(CitilinkMerchDto model, CitilinkMerchEntity entity)
        {
            EnsureCitilinkIdInitialized();
            entity.Name = model.Name;
            entity.PriceHistory = _priceHistoryMapper.Map(model.PriceTrack);
            entity.ShopId = _citilinkId ?? throw new InvalidOperationException();
            entity.CitilinkId = model.CitilinkId;

        }

        private void EnsureCitilinkIdInitialized()
        {
            if (_citilinkId != null)
                return;
            using var context = _contextFactory.CreateDbContext();
            _citilinkId = context.Shops.Single(s => s.Name == "Citilink").Id;
            if (_citilinkId == null)
                throw new InvalidOperationException();
        }
    }
}
