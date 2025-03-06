namespace PriceTracker.Models.BaseModels
{
    public interface IShopCollection: IShopSelector, IEnumerable<AbstractShop>
    {
        public bool AddShop(AbstractShop shop);
        public bool RemoveShopById(int id);
        public bool ChangeShopName(AbstractShop? shop, string newName);
    }
}
