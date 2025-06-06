using PriceTracker.Models.DomainModels;
using PriceTracker.Models.Utils;
using PriceTracker.Models.Services.MerchService;
using PriceTracker.Models.Services.Mapping.Extensions;
using PriceTracker.Models.Services.Mapping.MicroMappers;
using PriceTracker.Models.DTOModels.ForAPI.Shop;
using PriceTracker.Routing;
using PriceTracker.Modules.Repository.DataAccess.Repositories;


namespace PriceTracker.Models.Services.ShopService
{
    public class ShopService : IShopService
    {
        protected ILogger Logger;
        public ShopRepository Repository { get; init; }
        public List<ShopModel> Shops => Repository.GetAll();
        public ShopService(ILogger<Program> logger, ShopRepository repository)
        {
            Logger = logger;
            Repository = repository;
        }

        public ShopModel? GetShopByName(string name)
        {
            var shopModel = RepositoryUtil.TryGetSingle(Repository, shop => shop.Name == name,
                $"Не удалось выбрать магазин с name={name}, так как не получилось однозначно его выбрать.");
            return shopModel;
        }

        public ShopModel? GetShopById(int id)
        {
            var shopModel = RepositoryUtil.TryGetSingle(Repository, shop => shop.Id == id,
                $"Не удалось выбрать магазин с id={id}, так как не получилось однозначно его выбрать.");
            return shopModel;
        }
        public virtual bool AddShop(ShopModel shop)
        {

            if (IsShopUnique(shop))
            {
                Repository.Create(shop);
                Logger.LogInformation($"Добавлен магазин {shop.Name}");
                return true;
            }
            else
            {
                Logger.LogError($"Не удалось добавить магазин {shop.Name}: магазин с таким названием уже существует.");
                return false;
            }
        }
        public bool RemoveShopById(int id)
        {
            return RepositoryUtil.TryRemoveSingle(Repository, shop => shop.Id == id, $"Не удалось удалить магазин с id={id}");
        }

        public bool ChangeShopName(ShopModel shop, string newName)
        {
            if (IsNameUnique(newName) && shop != null)
            {
                shop.Name = newName;
                Repository.Update(shop);
                return true;
            }
            else
                return false;
        }

        protected bool IsShopUnique(ShopModel shop)
        {
            return IsNameUnique(shop.Name);
        }

        protected bool IsNameUnique(string name)
        {
            return !Repository.Any(s => s.Name == name);
        }

    }
}


