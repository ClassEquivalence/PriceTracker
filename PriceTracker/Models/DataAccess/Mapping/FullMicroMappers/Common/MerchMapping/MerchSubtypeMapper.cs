using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common.MerchMapping
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
