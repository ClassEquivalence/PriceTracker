using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository
{
    public class RepositoryInitializer
    {

        public void EnsureInitialized(ShopRepository repository)
        {
            var citilink = repository.SingleOrDefault(s => s.Name ==
            RepositoryParameters.CitilinkShopName);
            if (citilink == null)
            {
                citilink = new(default, RepositoryParameters.CitilinkShopName,
                    []);
                repository.Create(citilink);
            }
        }

    }
}
