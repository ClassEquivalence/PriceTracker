namespace PriceTracker.Models.BaseAppModels.PriceExtracting.ShopMerchExtracting
{
    public abstract class ShopPeriodicalPricesOverseer: IShopPeriodicalPricesOverseer
    {
        protected IShopMerchExtractor ShopMerchExtractor;
        protected IShopPriceExtractor ShopPriceExtractor;
        public ShopPeriodicalPricesOverseer(IShopMerchExtractor shopMerchExtractor, IShopPriceExtractor shopPriceExtractor) 
        {
            ShopMerchExtractor = shopMerchExtractor;
            ShopPriceExtractor = shopPriceExtractor;
        }
        public void Process()
        {
            Task.Run( async () => 
            {
                while (true)
                {
                    UpdateShopPrices();
                    await Task.Delay(Configs.PriceUpdatePeriod);
                }
            });
        }
        protected void UpdateShopPrices()
        {
            var list = ShopMerchExtractor.GetAllShopMerches();
            foreach (var sm in list)
            {
                var price = ShopPriceExtractor.Extract(sm);
                sm.PriceTrack.AddPrice(price);
            }
        }
    }
}
