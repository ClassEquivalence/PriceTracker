namespace PriceTracker.Models.BaseAppModels.ShopCollections
{
    public interface IShopCollection : IShopSelector, IEnumerable<Shop>
    {
        public bool AddShop(Shop shop);
        public bool RemoveShopById(int id);
        public bool ChangeShopName(Shop? shop, string newName);
    }
}
