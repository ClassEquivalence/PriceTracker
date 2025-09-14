using PriceTracker.Core.Models;
using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Mapping
{
    public abstract class BaseGenericCoreToEntityMapper<TCoreModel, TEntity> :
        ICoreToEntityMapper<TCoreModel, TEntity>
        where TCoreModel : BaseDto
        where TEntity : BaseEntity
    {
        public BaseGenericCoreToEntityMapper()
        {
        }

        public TCoreModel Map(TEntity entity)
        {
            return CreateModelFromEntity(entity);
        }

        public TEntity Map(TCoreModel model)
        {
            return CreateEntityFromModel(model);
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

        protected abstract void MapModelFieldsToEntity(TCoreModel model, TEntity entity);
    }
}
