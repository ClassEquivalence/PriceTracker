using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;
using System.Runtime.CompilerServices;
using PriceTracker.Models.DataAccess.Mapping.Extensions;

using MerchMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchModel, PriceTracker.Models.DataAccess.Entities.Domain.MerchEntity>;
using ShopMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.ShopModel, PriceTracker.Models.DataAccess.Entities.Domain.ShopEntity>;
using PriceHistoryMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchPriceHistory, PriceTracker.Models.DataAccess.Entities.Domain.MerchPriceHistoryEntity>;
using TimestampedPriceMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.TimestampedPrice, PriceTracker.Models.DataAccess.Entities.Domain.TimestampedPriceEntity>;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common;
using PriceTracker.Models.DataAccess.Mapping.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common.MerchMapping;
using PriceTracker.Models.DataAccess.Entities.Domain;



/*
 /// <summary>
        /// entity при попадании в метод должен впитать, отмаппить в себя содержимое model.
        /// Не реализуй новых инстансов в методе.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        protected abstract void ModelInwardsToEntity(TEntity entity, TDomain domain);

        /// <summary>
        /// С чистого листа создаётся entity на основе model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract TEntity CreateEntityFromModel(TDomain model);

        /// <summary>
        /// Создание инстанса Model на основе entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TDomain CreateDomainFromEntity(TEntity entity);
 
 */


// TODO: [Допущение] предполагается маппинг (и соответственно - загрузка в репозитории) всех навигационных свойств.
// TODO: У маппинга в нынешнем состоянии - проблемы с масштабируемостью. Желательно исправить.

namespace PriceTracker.Models.DataAccess.Mapping
{
    public class BidirectionalEntityModelMappingContext: IBidirectionalEntityModelMappingContext
    {


        private readonly MerchMapper _merchMapper;

        private readonly PriceHistoryMapper _priceHistoryMapper;
        private readonly ShopMapper _shopMapper;
        private readonly TimestampedPriceMapper _timestampedPriceMapper;

        public BidirectionalEntityModelMappingContext(
            MerchMapper merchMapper, PriceHistoryMapper priceHistoryMapper,
            ShopMapper shopMapper, TimestampedPriceMapper timestampedPriceMapper)
        {

            _timestampedPriceMapper = timestampedPriceMapper;
            _priceHistoryMapper = priceHistoryMapper;

            _merchMapper = merchMapper;

            _shopMapper = shopMapper;
            
            
        }

        public virtual MerchEntity Map(MerchModel model)
        {
            return _merchMapper.Map(model);
        }
        public virtual ShopEntity Map(ShopModel model)
        {
            return _shopMapper.Map(model);
        }
        public virtual TimestampedPriceEntity Map(TimestampedPrice model)
        {
            return _timestampedPriceMapper.Map(model);
        }
        public virtual MerchPriceHistoryEntity Map(MerchPriceHistory model)
        {
            return _priceHistoryMapper.Map(model);
        }

        public virtual MerchModel Map(MerchEntity entity)
        {
            return _merchMapper.Map(entity);
        }
        public virtual ShopModel Map(ShopEntity entity)
        {
            return _shopMapper.Map(entity);
        }
        public virtual TimestampedPrice Map(TimestampedPriceEntity entity)
        {
            return _timestampedPriceMapper.Map(entity);
        }
        public virtual MerchPriceHistory Map(MerchPriceHistoryEntity entity)
        {
            return _priceHistoryMapper.Map(entity);
        }

        
    }
}
