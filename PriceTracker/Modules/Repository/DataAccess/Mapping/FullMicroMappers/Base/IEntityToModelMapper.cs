using PriceTracker.Models;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{
    public interface IEntityToModelMapper<in TEntity, out TModel>
        where TEntity : BaseEntity where TModel : BaseModel
    {
        public TModel Map(TEntity entity);
    }
}
