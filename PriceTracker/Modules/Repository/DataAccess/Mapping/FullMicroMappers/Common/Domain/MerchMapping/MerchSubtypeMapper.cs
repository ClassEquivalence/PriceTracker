using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain.MerchMapping
{
    public abstract class MerchSubtypeMapper<TDomain, TEntity> : BidirectionalDomainEntityMapper<TDomain, TEntity>,
        IMerchSubtypeMapper<TDomain, TEntity>
        where TDomain : MerchModel where TEntity : MerchEntity
    {
        public MerchSubtypeMapper(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
