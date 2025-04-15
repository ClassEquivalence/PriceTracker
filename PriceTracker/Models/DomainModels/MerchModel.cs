using PriceTracker.Models.DataAccess.Entities;

namespace PriceTracker.Models.DomainModels
{
    public class MerchModel: BaseModel, IMerchModel
    {
        public string Name { get; set; }

        //в конструкторе следует инициализировать после CurrentPrice, если используется непустой конструктор.
        //Если конструктор пустой - то перед CurrentPrice.
        public MerchPriceHistory PriceTrack { get; init; }
        public TimestampedPrice CurrentPrice => PriceTrack.CurrentPrice;

        public int ShopId { get; set; }
        public ShopModel Shop { get; set; }

        //перед инициализацией проверить возможность присвоения имени.
        public MerchModel(string name, TimestampedPrice currentPrice, ShopModel shop, int id = default)
            : base(id)
        {
            Name = name;
            PriceTrack = new(currentPrice);
            Id = id;
            Shop = shop;
        }
        public MerchModel(string name, MerchPriceHistory priceTrack, ShopModel shop, int id = default)
            : base(id)
        {
            Name=name;
            PriceTrack = priceTrack;
            Shop = shop;
        }
    }
}


