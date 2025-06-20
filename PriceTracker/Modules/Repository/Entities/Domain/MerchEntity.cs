namespace PriceTracker.Modules.Repository.Entities.Domain
{
    public class MerchEntity : BaseEntity
    {
        public string Name { get; set; }
        public MerchPriceHistoryEntity PriceHistory { get; set; }
        public int PriceHistoryId { get; set; }

        public ShopEntity Shop { get; set; }
        public int ShopId { get; set; }

        // Исправить ситуацию с неинициализированными non nullable.
        public MerchEntity(string name, int id = default) :
            base(id)
        {
            Name = name;
        }
    }
}
