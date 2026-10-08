namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models.DeserializedMerchFetchResponse
{
    public class Response
    {
        public ResponseData data { get; set; }
        public Product[] GetProducts()
        {
            return data.productsFilter.record.products;
        }
        //response.data.productsFilter.record.products
    }
}
