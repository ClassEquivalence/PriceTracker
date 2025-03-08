namespace PriceTracker.Models.BaseModels
{
    public class ShopMerch: IShopMerch
    {
        public int Id { get; set; }
        protected string name;
        public string Name 
        { 
            get 
            {
                return name;
            }
            set
            {
                if (ValidateNameAvailability == null)
                    name = value;
                else if (ValidateNameAvailability(value))
                    name = value;
            }
        }

        //в конструкторе следует инициализировать после CurrentPrice, если используется непустой конструктор.
        //Если конструктор пустой - то перед CurrentPrice.
        public ShopMerchPriceTrack PriceTrack { get; set; }

        protected TimedPrice currentPrice;
        public TimedPrice CurrentPrice 
        {
            get
            {
                return currentPrice;
            }
            set
            {
                if(PriceTrack!=null)
                    PriceTrack.AddPrice(value);
                currentPrice = value;
            }
        }


        public Func<string, bool>? ValidateNameAvailability;

        //перед инициализацией проверить возможность присвоения имени.
        public ShopMerch(string name, decimal currentPrice, int id = default)
        {
            this.name = name;
            var timedPrice = new TimedPrice { DateTime = DateTime.Now, Price = currentPrice };
            CurrentPrice = timedPrice;
            PriceTrack = new(timedPrice);
            Id = id;
        }
    }
}
