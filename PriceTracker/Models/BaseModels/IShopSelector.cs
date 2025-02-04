namespace PriceTracker.Models.BaseModels
{
    public interface IShopSelector
    {
        public IShop? GetShopByName(string name);
        public IEnumerable<IShop> GetAll();
    }
}
