using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Repositories;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories.MerchRepository
{
    public interface IMerchSubtypeRepository<TDomain> : IRepository<TDomain>
    where TDomain : MerchModel
    {

    }
}
