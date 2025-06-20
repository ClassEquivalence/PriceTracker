using System.Security.Cryptography.X509Certificates;

namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop
{
    public record ShopOverviewDto : ShopNameDto
    {
        public string MerchesUrl { get; set; }
        public ShopOverviewDto(string name, string merchesUrl, int id = default) :
            base(name, id)
        {
            MerchesUrl = merchesUrl;
        }
    }
}
