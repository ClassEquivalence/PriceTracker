using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{
    public abstract class MerchSubtypeRepository<TDomain, TEntity> :
        EFGenericDomainRepository<TDomain, TEntity>,
        IMerchSubtypeRepository<TDomain>
        where TDomain : MerchDto
        where TEntity : MerchEntity
    {
        protected MerchSubtypeRepository(EFGenericEntityRepository<TEntity> entityRepository,
            ICoreToEntityMapper<TDomain, TEntity> mapper, ILogger? logger = null) :
            base(entityRepository, mapper, logger)
        {
        }
    }
}
