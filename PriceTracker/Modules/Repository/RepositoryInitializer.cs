using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository
{
    public class RepositoryInitializer
    {
        private readonly ShopRepository _shopRepository;
        private readonly IDbContextFactory<PriceTrackerContext> _factory;
        public RepositoryInitializer(ShopRepository repository,
            IDbContextFactory<PriceTrackerContext> factory)
        {
            _shopRepository = repository;
            _factory = factory;
        }
        public void EnsureInitialized()
        {
            using (var context = _factory.CreateDbContext())
            {
                context.Database.EnsureCreated();
            }
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
