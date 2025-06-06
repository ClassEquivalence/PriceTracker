using PriceTracker.Models;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{
    public interface IModelToEntityMapper<in TModel, out TEntity>
        where TEntity : BaseEntity where TModel : BaseModel
    {
        public TEntity Map(TModel model);
    }
}
