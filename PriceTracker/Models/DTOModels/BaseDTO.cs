namespace PriceTracker.Models.DTOModels
{
    public record BaseDTO
    {
        public int Id { get; set; }
        public BaseDTO(int id)
        {
            Id = id;
        }
    }
}
