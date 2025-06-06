using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Repositories;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories.MerchRepository
{
    public interface IMerchSubtypeRepositoryAdapter : IRepository<MerchModel>
    {
        public Type HandledType { get; init; }
    }
}
