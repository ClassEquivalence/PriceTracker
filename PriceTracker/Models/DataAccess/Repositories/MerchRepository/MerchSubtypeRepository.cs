using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common.MerchMapping;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Repositories.MerchRepository
{
    public abstract class MerchSubtypeRepository<TDomain, TEntity> :
        EFGenericRepository<TDomain, TEntity>,
        IMerchSubtypeRepository<TDomain>
        where TDomain : MerchModel
        where TEntity : MerchEntity
    {
        private readonly IMerchSubtypeMapper<TDomain, TEntity> _mapper;



        public MerchSubtypeRepository(IMerchSubtypeMapper<TDomain, TEntity> mapper,
            PriceTrackerContext dbContext) :
            base(dbContext)
        {
            _mapper = mapper;
        }

        protected override TDomain EntityToModel(TEntity entity)
        {
            return _mapper.Map(entity);
        }

        protected override TEntity ModelToEntity(TDomain model)
        {
            return _mapper.Map(model);
        }

    }
}
