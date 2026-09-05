using PriceTracker.Core.CommonModels.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{
    public interface IMerchSubtypeRepository<TDomain> : IDomainRepository<TDomain>
    where TDomain : MerchDto
    {
        public Task CreateManyAsync(List<TDomain> citilinkMerches);
    }
}
