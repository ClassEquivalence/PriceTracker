
using PriceTracker.Models.BaseModels.TemplateMethods;

using System.Collections;

namespace PriceTracker.Models.BaseModels
{
    public class ShopCollection : IShopCollection
    {
        protected ILogger Logger;
        protected ICollection<Shop> Shops;
        public ShopCollection(ILogger<Program> logger, ICollection<Shop>? shops = null)
        {
            Logger = logger;
            if (shops != null)
                Shops = shops;
            else
                Shops = new List<Shop>();
        }

        public Shop? GetShopByName(string name)
        {
            return CollectionSingleObjectController.TryGetSingle(Shops, shop => shop.Name == name,
                $"Не удалось выбрать магазин с name={name}, так как не получилось однозначно его выбрать.");
        }

        public Shop? GetShopById(int id)
        {
            return CollectionSingleObjectController.TryGetSingle(Shops, shop => shop.Id == id,
                $"Не удалось выбрать магазин с id={id}, так как не получилось однозначно его выбрать.");
        }
        public IEnumerable<Shop> GetAll()
        {
            return Shops;
        }
        public bool AddShop(Shop shop)
        {
            shop.Id = getFreeId();

            (bool sameNameExists, bool sameIdExists) = checkShopUniqueness(shop);
            if (!sameNameExists && !sameIdExists)
            {
                shop.ValidateNameAvailability = isNameUnique;
                Shops.Add(shop);
                Logger.LogInformation($"Добавлен магазин {shop.Name}");
                return true;
            }
            else
            {
                string shopFieldsExistenceInfo;
                switch (sameNameExists, sameIdExists)
                {
                    case (true, true):
                        shopFieldsExistenceInfo = "названием и Id";
                        break;
                    case (true, false):
                        shopFieldsExistenceInfo = "названием";
                        break;
                    case (false, true):
                        shopFieldsExistenceInfo = "Id";
                        break;
                    case (false, false):
                        Logger.LogError($"По неизвестным причинам не удалось добавить магазин {shop.Name}");
                        return false;
                }

                Logger.LogError($"Не удалось добавить магазин {shop.Name}: магазин с таким {shopFieldsExistenceInfo} уже существует");
                return false;
            }
        }
        public bool RemoveShopById(int id)
        {
            return CollectionSingleObjectController.TryRemoveSingle(Shops, shop => shop.Id == id, $"Не удалось удалить магазин с id={id}");
        }

        public bool ChangeShopName(Shop? shop, string newName)
        {
            if (isNameUnique(newName) && shop!=null)
            {
                shop.Name = newName;
                return true;
            }
            else
                return false;
        }

        protected (bool sameNameExist, bool sameIdExist) checkShopUniqueness(Shop shop)
        {
            bool sameNameExists, sameIdExists;
            sameNameExists = !isNameUnique(shop.Name);
            sameIdExists = !isIdUnique(shop.Id);
            return (sameNameExists, sameIdExists);
        }

        bool isNameUnique(string name)
        {
            return Shops.Where(s => s.Name == name).Count() > 0 ? false : true;
        }
        bool isIdUnique(int id)
        {
            return Shops.Where(s => s.Id == id).Count() > 0 ? false : true;
        }

        protected int AvailableId = 1;
        int getFreeId()
        {
            while (Shops.Any(shop => shop.Id == AvailableId))
            {
                AvailableId++;
            }
            return AvailableId;
        }

        public IEnumerator<Shop> GetEnumerator()
        {
            return Shops.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}


