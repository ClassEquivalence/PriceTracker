namespace PriceTracker.Models.BaseModels
{
    public interface IShopCollection: IShopSelector, IEnumerable<Shop>
    {
        public bool AddShop(Shop shop);
        public bool RemoveShopById(int id);
        public bool ChangeShopName(Shop? shop, string newName);
    }
}
