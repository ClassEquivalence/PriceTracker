using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DTOModels.ForAPI.Shop;

namespace PriceTracker.Models.Services.ShopService
{
    public interface IShopSelector
    {
        /*
         TODO:
        Взятие списка магазинов с именами
        Взятие конкретного магазина со ссылкой на коллекцию его товаров.
         */

        public ShopModel? GetShopByName(string name);
        public ShopModel? GetShopById(int id);
        public List<ShopModel> Shops { get; }


    }
}