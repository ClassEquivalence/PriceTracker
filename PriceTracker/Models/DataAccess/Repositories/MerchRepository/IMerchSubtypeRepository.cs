using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Repositories.MerchRepository
{
    public interface IMerchSubtypeRepository<TDomain> : IRepository<TDomain>
    where TDomain : MerchModel
    {

    }
}
