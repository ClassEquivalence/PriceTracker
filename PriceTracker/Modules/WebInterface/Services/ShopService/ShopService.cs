using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade;


namespace PriceTracker.Modules.WebInterface.Services.ShopService
{
    public class ShopService : IShopService
    {
        protected ILogger Logger;
        public IShopRepositoryFacade Repository { get; init; }

        public List<ShopDto> Shops => Repository.GetAll();

        public ShopService(ILogger<Program> logger, IShopRepositoryFacade repository)
        {
            Logger = logger;
            Repository = repository;
        }

        public ShopDto? GetShopByName(string name)
        {

            var shopModel = Repository.SingleOrDefault(s => s.Name == name);
            return shopModel;
        }

        public ShopDto? GetShopById(int id)
        {
            var shopModel = Repository.GetModel(id);
            return shopModel;
        }
        public virtual bool AddShop(ShopDto shop)
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
            return Repository.Delete(id);
        }

        public bool ChangeShopName(ShopDto shop, string newName)
        {
            if (IsNameUnique(newName) && shop != null)
            {
                ShopDto updated = new(shop.Id, newName, shop.Merches);
                Repository.Update(updated);
                return true;
            }
            else
                return false;
        }

        protected bool IsShopUnique(ShopDto shop)
        {
            return IsNameUnique(shop.Name);
        }

        protected bool IsNameUnique(string name)
        {
            return !Repository.Any(s => s.Name == name);
        }


    }
}


