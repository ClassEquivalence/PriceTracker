using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.Services.MerchService
{
    public interface IMerchService<Merch> where Merch : MerchModel
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
        public List<Merch> Merches { get; }
        public List<Merch> GetMerchesOfShop(int shopId);
        public Merch? GetMerch(int merchId);

        public bool TryCreate(Merch merch);
        public bool TryDelete(int merchId);
        public bool TryChangeName(int merchId, string newName);
        public bool TryAddTimestampedPrice(int merchId, TimestampedPrice timestampedPrice);
        public bool SetCurrentPrice(int merchId, decimal currentPrice);
        public bool RemoveSingleTimestampedPrice(int timestampedPriceId);
        public bool ClearOldPrices(int merchId);
    }
    public interface IMerchService: IMerchService<MerchModel>
    {

    }
}
