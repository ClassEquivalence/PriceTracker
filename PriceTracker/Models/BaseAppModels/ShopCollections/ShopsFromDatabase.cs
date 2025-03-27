using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DbRelatedModels;

namespace PriceTracker.Models.BaseAppModels.ShopCollections
{
    public class ShopsFromDatabase : ShopCollection
    {
        public ShopsFromDatabase(ILogger<Program> logger, PriceTrackerContext context): 
            base(logger, new DbDataExtractor<Shop>(context))
        {
        }
        public override bool AddShop(Shop shop)
        {
            bool sameNameExists = isNameUnique(shop.Name);
            if (!sameNameExists)
            {
                shop.ValidateNameAvailability = isNameUnique;
                Shops.Add(shop);
                Logger.LogInformation($"Добавлен магазин {shop.Name}");
                return true;
            }
            else
            {
                Logger.LogError($"Не удалось добавить магазин {shop.Name}: магазин с таким названием уже существует");
                return false;
            }
        }
    }
}
