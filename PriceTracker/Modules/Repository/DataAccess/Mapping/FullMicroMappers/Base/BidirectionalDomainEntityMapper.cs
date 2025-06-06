using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{


    // TODO: В целях масштабируемости можно выделить ветку иерархии,
    // отвечающую не за создание новых моделей, а за маппинг всех свойств
    // (на основе и по аналогии с MapModelFieldsToEntity).
    public abstract class BidirectionalDomainEntityMapper<TDomain, TEntity> :
        BidirectionalModelEntityMapper<TDomain, TEntity>,
        IBidirectionalDomainEntityMapper<TDomain, TEntity>
        where TDomain : BaseDomain where TEntity : BaseEntity
    {


        public BidirectionalDomainEntityMapper(DbContext dbContext) : base(dbContext)
        {

        }
        public BidirectionalDomainEntityMapper(DbContext dbContext,
            Dictionary<int, WeakReference<TDomain>> cache)
            : base(dbContext, cache)
        {

        }
    }
}
