using MerchMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchModel, PriceTracker.Models.DataAccess.Entities.Domain.MerchEntity>;
using ShopMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.ShopModel, PriceTracker.Models.DataAccess.Entities.Domain.ShopEntity>;
using PriceHistoryMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchPriceHistory, PriceTracker.Models.DataAccess.Entities.Domain.MerchPriceHistoryEntity>;
using TimestampedPriceMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.TimestampedPrice, PriceTracker.Models.DataAccess.Entities.Domain.TimestampedPriceEntity>;




namespace PriceTracker.Models.DataAccess.Mapping
{
    public interface IBidirectionalEntityModelMappingContext: MerchMap, ShopMap, PriceHistoryMap, TimestampedPriceMap
    {
    }
}
