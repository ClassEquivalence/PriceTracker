using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Repositories.MerchRepository
{
    public interface IMerchSubtypeRepositoryAdapter : IRepository<MerchModel>
    {
        public Type HandledType { get; init; }
    }
}
