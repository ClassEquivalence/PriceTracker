using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain.MerchMapping;
using PriceTracker.Modules.Repository.DataAccess.Repositories;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories.MerchRepository
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
