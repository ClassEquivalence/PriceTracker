using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{
    public interface IMerchSubtypeRepository<TDomain> : IDomainRepository<TDomain>
    where TDomain : MerchDto
    {

    }
}
