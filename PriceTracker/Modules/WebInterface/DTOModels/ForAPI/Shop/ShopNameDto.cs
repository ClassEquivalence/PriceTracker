namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop
{
    public record ShopNameDto : BaseDomainWebInterfaceDto
    {
        public string Name { get; set; }

        public ShopNameDto(string name, int id = default) : base(id)
        {
            Name = name;
        }
    }
}
