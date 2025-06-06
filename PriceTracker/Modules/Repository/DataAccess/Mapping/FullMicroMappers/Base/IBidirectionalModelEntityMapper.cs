using PriceTracker.Models;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{
    public interface IBidirectionalModelEntityMapper<TModel, TEntity>
        : IEntityToModelMapper<TEntity, TModel>,
        IModelToEntityMapper<TModel, TEntity>
        where TModel : BaseModel where TEntity : BaseEntity
    {

    }
}
