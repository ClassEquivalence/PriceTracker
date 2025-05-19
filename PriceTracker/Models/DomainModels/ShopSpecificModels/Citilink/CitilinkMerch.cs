namespace PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink
{
    public class CitilinkMerch: MerchModel
    {
        public string CitilinkId { get; set; }
        public CitilinkMerch(string name, MerchPriceHistory priceHistory, 
            ShopModel shop, string citilinkId, int id = default): base(name, priceHistory, shop, id)
        {
            CitilinkId = citilinkId;
        }
    }
}
