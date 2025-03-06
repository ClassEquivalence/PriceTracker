namespace PriceTracker.Models.BaseModels
{
    public interface IShopSelector
    {
        public AbstractShop? GetShopByName(string name);
        public AbstractShop? GetShopById(int id);
        public IEnumerable<AbstractShop> GetAll();
    }
}
