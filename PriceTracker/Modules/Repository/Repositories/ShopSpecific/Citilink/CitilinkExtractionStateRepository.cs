using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    // TODO: Класс не наследует другие более общие классы, или интерфейсы.
    // Не входит в иерархию. Соответственно это костыль который можно переделать.
    // Ещё и маппинг прямо здесь реализован, что в теории перегружает ответственностью.

    // Ещё в отличие от других репозиториев уровня сущности, этот использует 
    // DbContext один на весь репозиторий, из-за чего при расширении его использования
    // до многопоточного к нему доступа могут возникнуть проблемы. Уязвимое место!
    public class CitilinkExtractionStateRepository
    {
        private readonly PriceTrackerContext _dbContext;
        private readonly DbSet<CitilinkParsingExecutionStateEntity> _execState;
        public CitilinkExtractionStateRepository(PriceTrackerContext dbContext)
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

        public CitilinkExtractionStateDto Provide()
        {
            var entity = _execState.Single();
            CitilinkExtractionStateDto stateDto = new(entity.IsCompleted, 
                entity.CurrentCatalogUrl, entity.CatalogPageNumber);
            return stateDto;
        }

        public void Save(CitilinkExtractionStateDto info)
        {
            var entity = _execState.Single();
            entity.CurrentCatalogUrl = info.CurrentCatalogUrl;
            entity.CatalogPageNumber = info.CatalogPageNumber;
            entity.IsCompleted = info.IsCompleted;
            _dbContext.SaveChanges();
        }

    }
}
