using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository
{
    public class RepositoryInitializer
    {
        private readonly ShopRepository _shopRepository;
        public RepositoryInitializer(ShopRepository repository)
        {
            _shopRepository = repository;
        }

        public void EnsureInitialized()
        {
            var citilink = _shopRepository.SingleOrDefault(s => s.Name ==
            RepositoryParameters.CitilinkShopName);
            if (citilink == null)
            {
                citilink = new(default, RepositoryParameters.CitilinkShopName,
                    []);
                _shopRepository.Create(citilink);
            }
        }

    }
}
