using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base
{
    public interface IModelToEntityMapper<in TModel, out TEntity>
        where TEntity : BaseEntity where TModel : BaseModel
    {
        public TEntity Map(TModel model);
    }
}
