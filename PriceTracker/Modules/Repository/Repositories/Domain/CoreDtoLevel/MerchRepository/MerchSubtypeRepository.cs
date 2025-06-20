using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
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
            ICoreToEntityMapper<TDomain, TEntity> mapper) : base(entityRepository, mapper)
        {
        }
    }
}
