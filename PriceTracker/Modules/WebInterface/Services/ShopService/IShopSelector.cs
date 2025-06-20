

using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.WebInterface.Services.ShopService
{
    public interface IShopSelector
    {
        /*
         TODO:
        Взятие списка магазинов с именами
        Взятие конкретного магазина со ссылкой на коллекцию его товаров.
         */

        public ShopDto? GetShopByName(string name);
        public ShopDto? GetShopById(int id);
        public List<ShopDto> Shops { get; }


    }
}