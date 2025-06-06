using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.ShopSpecific.Citilink
{
    public class CitilinkExtractionExecutionStateMapping :
        BidirectionalModelEntityMapper<CitilinkParsingExecutionState, CitilinkParsingExecutionStateEntity>
    {

        public CitilinkExtractionExecutionStateMapping(DbContext dbContext) :
            base(dbContext)
        {

        }

        protected override CitilinkParsingExecutionStateEntity
            CreateEntityFromModel(CitilinkParsingExecutionState state)
        {
            return new CitilinkParsingExecutionStateEntity(state.CurrentCatalogUrl,
                state.CatalogPageNumber, state.Id);
        }

        protected override CitilinkParsingExecutionState
            CreateModelFromEntity(CitilinkParsingExecutionStateEntity entity)
        {
            return new CitilinkParsingExecutionState(entity.CurrentCatalogUrl,
                entity.CatalogPageNumber);
        }

        protected override void MapModelFieldsToEntity(CitilinkParsingExecutionStateEntity entity,
            CitilinkParsingExecutionState state)
        {
            entity.CatalogPageNumber = state.CatalogPageNumber;
            entity.CurrentCatalogUrl = state.CurrentCatalogUrl;
        }
    }
}
