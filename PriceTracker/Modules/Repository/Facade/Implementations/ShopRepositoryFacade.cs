using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class ShopRepositoryFacade : IShopRepositoryFacade
    {
        private readonly IDbContextFactory<PriceTrackerContext> _contextFactory;

        public ShopRepositoryFacade(IDbContextFactory<PriceTrackerContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task EnsureCitilinkInitializedAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var citilink = await context.Shops.SingleOrDefaultAsync(s => s.Name == "Citilink");
            if(citilink is null)
            {
                context.Shops.Add(new("Citilink"));
            }
        }
    }
}
