
namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline
{
    public enum CitilinkNodeAugmenterAugmentationState
    {
        Success, Failure_PreviousLayer, NoUnprocessedLocationsLeft
    }
    public class CitilinkNodeAugmentMetadata
    {
        public CitilinkNodeAugmentMetadata(CitilinkNodeAugmenterAugmentationState state)
        {
            State = state;
        }

        public CitilinkNodeAugmenterAugmentationState State;

    }
}
