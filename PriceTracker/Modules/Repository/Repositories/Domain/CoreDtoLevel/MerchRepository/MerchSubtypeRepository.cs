using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{
    public abstract class MerchSubtypeRepository<TDomain, TEntity, SpecificDbContext> :
        EFGenericDomainRepository<TDomain, TEntity, SpecificDbContext>,
        IMerchSubtypeRepository<TDomain>
        where TDomain : MerchDto
        where TEntity : MerchEntity
        where SpecificDbContext : DbContext
    {
        protected MerchSubtypeRepository(IEntityRepository<TEntity> entityRepository,
            ICoreToEntityMapper<TDomain, TEntity> mapper, ILogger? logger = null) :
            base(entityRepository, mapper, logger)
        {
        }

        public abstract Task CreateManyAsync(List<TDomain> citilinkMerches);
    }
}
