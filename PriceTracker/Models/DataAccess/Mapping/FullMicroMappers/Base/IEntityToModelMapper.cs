using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base
{
    public interface IEntityToModelMapper<in TEntity, out TModel>
        where TEntity: BaseEntity where TModel: BaseModel
    {
        public TModel Map(TEntity entity);
    }
}
