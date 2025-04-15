namespace PriceTracker.Models.DTOModels
{
    public record TimestampedPriceDTO: BaseDTO
    {
        public decimal Price { get; init; }
        public DateTime DateTime { get; init; }
        public TimestampedPriceDTO(decimal price, DateTime dateTime, int id = default)
            : base(id)
        {
            Price = price;
            DateTime = dateTime;
        }
    }
}
