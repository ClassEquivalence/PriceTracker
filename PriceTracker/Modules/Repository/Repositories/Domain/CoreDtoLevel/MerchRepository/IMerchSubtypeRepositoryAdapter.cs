using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{
    public interface IMerchSubtypeRepositoryAdapter : IDomainRepository<MerchDto>
    {
        public Type HandledType { get; }
    }
}
