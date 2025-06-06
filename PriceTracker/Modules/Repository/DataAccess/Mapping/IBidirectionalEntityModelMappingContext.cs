using MerchMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchModel, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.MerchEntity>;
using ShopMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.ShopModel, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.ShopEntity>;
using PriceHistoryMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchPriceHistory, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.MerchPriceHistoryEntity>;
using TimestampedPriceMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.TimestampedPrice, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.TimestampedPriceEntity>;




namespace PriceTracker.Modules.Repository.DataAccess.Mapping
{
    public interface IBidirectionalEntityModelMappingContext : MerchMap, ShopMap, PriceHistoryMap, TimestampedPriceMap
    {
    }
}
