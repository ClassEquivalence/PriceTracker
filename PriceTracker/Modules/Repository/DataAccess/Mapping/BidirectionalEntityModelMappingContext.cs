using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using System.Runtime.CompilerServices;
using PriceTracker.Models.DataAccess.Mapping.Extensions;

using MerchMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchModel, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.MerchEntity>;
using ShopMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.ShopModel, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.ShopEntity>;
using PriceHistoryMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.MerchPriceHistory, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.MerchPriceHistoryEntity>;
using TimestampedPriceMap = PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base.IBidirectionalDomainEntityMapper
    <PriceTracker.Models.DomainModels.TimestampedPrice, PriceTracker.Modules.Repository.DataAccess.Entities.Domain.TimestampedPriceEntity>;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Entities.Process.ShopSpecific;
using PriceTracker.Modules.Repository.DataAccess.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain.MerchMapping;




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

namespace PriceTracker.Modules.Repository.DataAccess.Mapping
{
    public class BidirectionalEntityModelMappingContext : IBidirectionalEntityModelMappingContext
    {


        private readonly MerchMapper _merchMapper;

        private readonly PriceHistoryMapper _priceHistoryMapper;
        private readonly ShopMapper _shopMapper;
        private readonly TimestampedPriceMapper _timestampedPriceMapper;
        private readonly CitilinkMerchMapper _citilinkMerchMapper;

        public BidirectionalEntityModelMappingContext(
            MerchMapper merchMapper, PriceHistoryMapper priceHistoryMapper,
            ShopMapper shopMapper, TimestampedPriceMapper timestampedPriceMapper,
            CitilinkMerchMapper citilinkMerchMapper)
        {

            _timestampedPriceMapper = timestampedPriceMapper;
            _priceHistoryMapper = priceHistoryMapper;

            _merchMapper = merchMapper;

            _shopMapper = shopMapper;

            _citilinkMerchMapper = citilinkMerchMapper;

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

        public virtual CitilinkMerchEntity Map(CitilinkMerch model)
        {
            return _citilinkMerchMapper.Map(model);
        }
        public virtual CitilinkMerch Map(CitilinkMerchEntity entity)
        {
            return _citilinkMerchMapper.Map(entity);
        }

    }
}
