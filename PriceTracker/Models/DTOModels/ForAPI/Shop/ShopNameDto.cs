namespace PriceTracker.Models.DTOModels.ForAPI.Shop
{
    public record ShopNameDto: BaseDTO
    {
        public string Name { get; set; }

        public ShopNameDto(string name, int id=default): base(id)
        {
            Name = name;
        }
    }
}
