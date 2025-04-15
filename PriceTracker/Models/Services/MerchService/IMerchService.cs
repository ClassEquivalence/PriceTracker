using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.Services.MerchService
{
    public interface IMerchService
    {
        /*
         TODO:
        Считывание всех товаров
        Считывание товаров выбранного магазина
        Подробное считывание конкретного товара

        Выбор товара по... артикулу + id магазина?
        Или иному уникальному признаку + id магазина?

        Создание товара
        Удаление товара
        Изменение товара
         

         */
        public List<MerchModel> Merches { get; }
        public List<MerchModel> GetMerchesOfShop(int shopId);
        public MerchModel? GetMerch(int merchId);

        public bool TryCreate(MerchModel merch);
        public bool TryDelete(int merchId);
        public bool TryChangeName(int merchId, string newName);
        public bool TryAddTimestampedPrice(int merchId, TimestampedPrice timestampedPrice);
        public bool SetCurrentPrice(int merchId, decimal currentPrice);
        public bool RemoveSingleTimestampedPrice(int timestampedPriceId);
        public bool ClearOldPrices(int merchId);
    }
}
