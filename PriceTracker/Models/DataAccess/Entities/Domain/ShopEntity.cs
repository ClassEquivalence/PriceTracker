using PriceTracker.Models.DTOModels;

namespace PriceTracker.Models.DataAccess.Entities.Domain
{
    public class ShopEntity : BaseEntity
    {
        public string Name { get; set; }
        public virtual List<MerchEntity> Merches { get; set; }

        // разобраться с non nullable.
        public ShopEntity(string name, int id = default)
            : base(id)
        {
            Name = name;
        }
    }
}
