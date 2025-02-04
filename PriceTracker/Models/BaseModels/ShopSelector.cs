namespace PriceTracker.Models.BaseModels
{
    public class ShopSelector: IShopSelector
    {
        protected ILogger Logger;
        protected IEnumerable<IShop> Shops;
        public ShopSelector(ILogger logger, IEnumerable<IShop> shops)
        {
            Logger = logger;
            Shops = shops;
        }

        public IShop? GetShopByName(string name)
        {
            try
            {
                var shop = Shops.Where(shop => shop.Name == name).SingleOrDefault();
                return shop;
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"Не удалось выбрать магазин с name={name}, так как не получилось однозначно его выбрать.");
                return null;
            }
        }
        public IEnumerable<IShop> GetAll()
        {
            return Shops;
        }
        public void AddShop(IShop shop)
        {
            Shops.Append(shop);
        }
        public void RemoveShopById(int id)
        {
            /*
            IShop shop;
            try
            {
                shop = Shops.Where(shop => shop.Id == id).Single();
                Shops.
                Shops.Remove(shop);
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"Не удалось удалить магазин с Id={id}, так как не получилось однозначно его выбрать.");
            }
            */
        }
    }
}


