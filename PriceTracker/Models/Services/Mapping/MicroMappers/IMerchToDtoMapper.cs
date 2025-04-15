using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels.ForAPI.Merch;

namespace PriceTracker.Models.Services.Mapping.MicroMappers
{
    public interface IMerchToDtoMapper
    {
        public MerchOverviewDto ToMerchOverview(MerchModel merch);
        public DetailedMerchDto ToDetailedMerch(MerchModel merch);
    }
}
