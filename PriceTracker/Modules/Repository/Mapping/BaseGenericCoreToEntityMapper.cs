using PriceTracker.Core.Models;
using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Mapping
{
    public abstract class BaseGenericCoreToEntityMapper<TCoreModel, TEntity> :
        ICoreToEntityMapper<TCoreModel, TEntity>
        where TCoreModel : BaseDto
        where TEntity : BaseEntity
    {
        private readonly Func<TCoreModel, TEntity?> _getEntityIfExists;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getEntityIfExists">Должен возвращать entity из репозитория,
        /// если эта entity существует. Если не существует - null.</param>
        public BaseGenericCoreToEntityMapper(Func<TCoreModel, TEntity?> getEntityIfExists)
        {
            _getEntityIfExists = getEntityIfExists;
        }

        public TCoreModel Map(TEntity entity)
        {
            return CreateModelFromEntity(entity);
        }

        public TEntity Map(TCoreModel model)
        {
            return _getEntityIfExists(model) ?? CreateEntityFromModel(model);
        }

        /// <summary>
        /// Создание Entity на основе модели. Создавать только основную сущность,
        /// связанные - маппить мапперами соответствующей сущности.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract TEntity CreateEntityFromModel(TCoreModel model);

        /// <summary>
        /// Создание модели на основе Entity. Создавать только основную сущность,
        /// связанные - маппить мапперами соответствующей сущности.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TCoreModel CreateModelFromEntity(TEntity entity);
    }
}
