namespace PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific
{
    public class CitilinkMerchEntity : MerchEntity
    {
        public string CitilinkId { get; set; }

        // Исправить ситуацию с неинициализированными non nullable.
        public CitilinkMerchEntity(string name, string citilinkId,
            int id = default) : base(name, id)
        {
            CitilinkId = citilinkId;
        }

        public override string ToString()
        {
            return $"{this.GetType()}: Name = {Name}, CitilinkId = {CitilinkId}, " +
                $"Id = {Id}, ShopId = {ShopId}, PriceHistoryId = {PriceHistory.Id}";
        }

    }
}
