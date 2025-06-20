using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public interface IMerchCoreToEntityMapper<TCoreModel, TEntity> :
        ICoreToEntityMapper<TCoreModel, TEntity>
        where TCoreModel : MerchDto where TEntity : MerchEntity
    {
    }
}
