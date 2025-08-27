using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    // TODO: Класс не наследует другие более общие классы, или интерфейсы.
    // Не входит в иерархию. Соответственно это костыль который можно переделать.
    // Ещё и маппинг прямо здесь реализован, что в теории перегружает ответственностью.

    // Ещё в отличие от других репозиториев уровня сущности, этот использует 
    // DbContext один на весь репозиторий, из-за чего при расширении его использования
    // до многопоточного к нему доступа могут возникнуть проблемы. Уязвимое место!
    public class CitilinkExtractionStateRepository: EFGenericSingletonRepository<
        CitilinkExtractionStateDto, CitilinkParsingExecutionStateEntity>
    {

        public CitilinkExtractionStateRepository(IDbContextFactory<PriceTrackerContext>
            factory, ICoreToEntityMapper<CitilinkExtractionStateDto, 
                CitilinkParsingExecutionStateEntity> mapper):
            base(factory, mapper)
        {

        }



        protected override IQueryable<CitilinkParsingExecutionStateEntity> 
            GetWithIncludedEntities(DbSet<CitilinkParsingExecutionStateEntity> entities)
        {
            return entities.Include(e => e.CatalogUrls).ThenInclude(cu => cu.Root)
                .ThenInclude(r => r.Branches).ThenInclude(b => b.Branches);
        }
    }
}
