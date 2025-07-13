using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities;
using PriceTracker.Modules.Repository.Mapping;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public abstract class EFGenericDomainRepository<TDomain, TEntity,
        SpecificDbContext> :
        EFGenericRepository<TDomain, TEntity, SpecificDbContext>,
        IDomainRepository<TDomain>
        where TEntity : BaseEntity where TDomain : DomainDto
        where SpecificDbContext : DbContext
    {
        protected EFGenericDomainRepository(IEntityRepository<TEntity>
            entityRepository,
            ICoreToEntityMapper<TDomain, TEntity> mapper, ILogger? logger = null) :
            base(entityRepository, mapper, logger)
        {

        }

        public TDomain? GetModel(int id)
        {
            return SingleOrDefault(dom => dom.Id == id);
        }


    }
}
