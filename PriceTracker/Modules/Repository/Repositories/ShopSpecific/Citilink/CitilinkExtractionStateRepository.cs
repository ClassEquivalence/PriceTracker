using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState;
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
            factory, ICitilinkExtractionStateMapper mapper):
            base(factory, mapper)
        {

        }



        protected override IQueryable<CitilinkParsingExecutionStateEntity> 
            GetWithIncludedEntities(DbSet<CitilinkParsingExecutionStateEntity> entities)
        {
            return entities.Include(e => e.CatalogUrls).ThenInclude(cu => cu.Root)
                .ThenInclude(r => r.Branches).ThenInclude(b => b.Branches);
        }

        public override void Set(CitilinkExtractionStateDto dto)
        {
            base.Set(dto);


            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<CitilinkParsingExecutionStateEntity>();
            var entity = _mapper.Map(dto);

            using var context_second = _dbContextFactory.CreateDbContext();

            var count = set.Count();
            if (count == 1)
            {
                entity.Id = set.Single().Id;
                context_second.Set<CitilinkParsingExecutionStateEntity>().Update(entity);


                if (entity.CatalogUrls != null)
                {
                    CleanRelatedEntityGarbage<CitilinkCatalogUrlsTreeEntity>(context_second,
                    [entity.CatalogUrls.Id]);
                }
                else
                {
                    CleanRelatedEntityGarbage<CitilinkCatalogUrlsTreeEntity>(context_second,
                        []);
                }

                CleanRelatedEntityGarbage<CitilinkCatalogBranchEntity>(context_second, dto.CachedUrls?.
                    GetAllBranches().Select(b=>b.Id).ToArray() ?? []);


                context_second.SaveChanges();
            }
            else if (count == 0)
            {
                context_second.Set<CitilinkParsingExecutionStateEntity>().Add(entity);
                context_second.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException($"{nameof(CitilinkExtractionStateRepository)}, " +
                    $"{nameof(Set)}: \n" +
                    $"Синглтон-сущностей типа {nameof(CitilinkParsingExecutionStateEntity)} почему-то не 1 и не 0.");
            }


        }


        /// <summary>
        /// ВАЖНО: idsToSpare указывают на те сущности, которые удалены не будут.
        /// Все остальные данного типа удалены будут.
        /// <br/>
        /// ВАЖНО: Необходимо вызвать SaveChanges после вызова метода вручную.
        /// </summary>
        /// <typeparam name="TCleanableEntity"></typeparam>
        /// <param name="idsToSpare"></param>
        private void CleanRelatedEntityGarbage<TCleanableEntity>(DbContext context, int[] idsToSpare)
            where TCleanableEntity: BaseEntity
        {
            var set = context.Set<TCleanableEntity>();
            var toRemove = set.Where(e => !idsToSpare.Contains(e.Id));
            set.RemoveRange(toRemove);
        }

    }
}
