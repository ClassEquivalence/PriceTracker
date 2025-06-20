using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction
{
    public class CitilinkParsingExecutionStateEntity : BaseEntity
    {
        public string CurrentCatalogUrl { get; set; }

        public int CatalogPageNumber { get; set; }

        public bool IsResumed { get; set; }

        public CitilinkParsingExecutionStateEntity(string currentCatalogUrl,
            int catalogPageNumber, bool isResumed, int id = default) : base(id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = catalogPageNumber;
            IsResumed = isResumed;
        }
    }
}
