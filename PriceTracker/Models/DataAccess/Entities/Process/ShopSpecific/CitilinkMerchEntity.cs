using PriceTracker.Models.DataAccess.Entities.Domain;

namespace PriceTracker.Models.DataAccess.Entities.Process.ShopSpecific
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


    }
}
