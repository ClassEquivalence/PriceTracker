using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public interface ICitilinkExtractionStateMapper: ICoreToEntityMapper
        <CitilinkExtractionStateDto, CitilinkParsingExecutionStateEntity>
    {
    }
}
