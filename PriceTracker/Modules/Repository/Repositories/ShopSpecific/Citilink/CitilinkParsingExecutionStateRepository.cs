using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Process;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    // TODO: Класс не наследует другие более общие классы, или интерфейсы.
    // Не входит в иерархию. Соответственно это костыль который можно переделать.
    // Ещё и маппинг прямо здесь реализован, что в теории перегружает ответственностью.
    public class CitilinkParsingExecutionStateRepository
    {
        private readonly PriceTrackerContext _dbContext;
        private readonly DbSet<CitilinkParsingExecutionStateEntity> _execState;
        public CitilinkParsingExecutionStateRepository(PriceTrackerContext dbContext)
        {
            _dbContext = dbContext;
            _execState = dbContext.CitilinkParsingExecutionStateEntity;
            if (!_execState.Any())
            {
                _execState.Add(new("", 0, false));
                _dbContext.SaveChanges();
            }
            else if (_execState.Count() > 1)
                throw new InvalidOperationException("Строк CitilinkParsingExecutionStateEntity" +
                    "в БД должно быть ровно 1.");

        }

        public CitilinkParsingExecutionState Provide()
        {
            var entity = _execState.Single();
            CitilinkParsingExecutionState stateDto = new(entity.CurrentCatalogUrl,
                entity.CatalogPageNumber, entity.IsResumed);
            return stateDto;
        }

        public void Save(CitilinkParsingExecutionState info)
        {
            var entity = _execState.Single();
            entity.CurrentCatalogUrl = info.CurrentCatalogUrl;
            entity.CatalogPageNumber = info.CatalogPageNumber;
            entity.IsResumed = info.IsResumed;
            _dbContext.SaveChanges();
        }

    }
}
