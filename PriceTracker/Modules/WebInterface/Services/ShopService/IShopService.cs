
using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.WebInterface.Services.ShopService
{
    public interface IShopService : IShopSelector
    {
        /*
         TODO:
        Создание магазина
        Изменение названия магазина
        Удаление магазина
         */
        public bool AddShop(ShopDto shop);
        public bool RemoveShopById(int id);
        public bool ChangeShopName(ShopDto shop, string newName);

    }
}
