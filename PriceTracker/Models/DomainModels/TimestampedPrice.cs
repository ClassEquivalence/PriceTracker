using PriceTracker.Models.DataAccess.Entities;

namespace PriceTracker.Models.DomainModels
{
    public class TimestampedPrice : BaseModel
    {
        public TimestampedPrice(decimal price, DateTime dateTime, int id = default): base(id) 
        { 
            Price = price;
            DateTime = dateTime;
        }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        public static TimestampedPrice CreateCurrentPrice(decimal price, int id = default)
        {
            TimestampedPrice timedPrice = new(price, DateTime.Now, id);
            return timedPrice;
        }
    }
}
