using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Entities.Process.ShopSpecific.Extraction
{
    public class CitilinkParsingExecutionStateEntity : BaseEntity
    {
        public string CurrentCatalogUrl { get; set; }

        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionStateEntity(string currentCatalogUrl,
            int catalogPageNumber, int id = default) : base(id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = catalogPageNumber;
        }
    }
}
