using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker.Modules.Repository
{
    public class RepositoryInitializer
    {
        private readonly IShopRepositoryFacade _shopRepository;
        private readonly IDbContextFactory<PriceTrackerContext> _factory;
        public RepositoryInitializer(IShopRepositoryFacade repository,
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
            // Костыль! Wait лучше не использовать обычно!
            _shopRepository.EnsureCitilinkInitializedAsync().Wait();
        }
    }
}
