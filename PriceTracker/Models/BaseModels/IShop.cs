using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PriceTracker.Models.BaseModels
{
    public interface IShop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<IShopMerch> GetAllMerches();
        public IShopMerch GetMerch(int id);
    }
}
