namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models
{
    public record CitilinkMerchParsingDto(decimal Price, string CitilinkId, string Name)
    {
        public override string ToString()
        {
            return $"Name: {Name}, Price: {Price}, CitilinkId: {CitilinkId}";
        }
    }
}
