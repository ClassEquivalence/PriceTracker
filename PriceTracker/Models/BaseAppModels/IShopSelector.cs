namespace PriceTracker.Models.BaseAppModels
{
    public interface IShopSelector
    {
        public Shop? GetShopByName(string name);
        public Shop? GetShopById(int id);
        public IEnumerable<Shop> AllShops { get; }



    }
}
