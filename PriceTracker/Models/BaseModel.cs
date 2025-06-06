namespace PriceTracker.Models
{
    public class BaseModel(int Id = default)
    {
        public int Id { get; set; } = Id;
    }
}
