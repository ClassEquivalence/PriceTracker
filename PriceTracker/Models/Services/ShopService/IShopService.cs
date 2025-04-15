using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Repositories;
using PriceTracker.Models.DTOModels;
using PriceTracker.Models.Services.Mapping;
using PriceTracker.Models.DTOModels.ForAPI.Shop;

namespace PriceTracker.Models.Services.ShopService
{
    public interface IShopService : IShopSelector
    {
        /*
         TODO:
        Создание магазина
        Изменение названия магазина
        Удаление магазина
         */
        public bool AddShop(ShopModel shop);
        public bool RemoveShopById(int id);
        public bool ChangeShopName(ShopModel shop, string newName);

    }
}
