using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities;
using PriceTracker.Modules.Repository.Mapping;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public abstract class EFGenericDomainRepository<TDomain, TEntity> :
        EFGenericRepository<TDomain, TEntity>,
        IDomainRepository<TDomain>
        where TEntity : BaseEntity where TDomain : DomainDto
    {
        protected EFGenericDomainRepository(EFGenericEntityRepository<TEntity> entityRepository,
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
