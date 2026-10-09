using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public class NewCitilinkCatalogTreeRepositoryAdapter : ICitilinkCatalogTreeRepositoryAdapter
    {
        // ХАРДКОД КОНСТАНТА! Вынести бы наверное.
        private const string mainCatalogUrl = "https://www.citilink.ru/catalog/";
        private readonly ILogger? _logger;
        private List<CitilinkNode> _retreivedNodeCache;
        private readonly IDbContextFactory<PriceTrackerContext> _dbContextFactory;

        public NewCitilinkCatalogTreeRepositoryAdapter(ILogger? logger,
            IDbContextFactory<PriceTrackerContext> factory)
        {
            _logger = logger;
            _retreivedNodeCache = [];

            _dbContextFactory = factory;
        }

        /// <summary>
        /// Метод добавляет для данного parent наследников descendants, синхронизируя parent с БД.
        /// </summary>
        /// /// <remarks>
        /// ВНИМАНИЕ: Это единственный легитимный способ добавления наследников в граф нод.
        /// Прямое добавление в обход этого метода нарушает целостность данных и приводит к 
        /// <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <param name="parent"></param>
        /// <param name="descendants"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task AppendBranchesAsync(CitilinkNode parent,
            List<CitilinkScrapeTarget> descendants)
        {
            // Уязвимость: Race condition? Переделать и возможно защитить, если она
            // гипотетически сможет себя проявить.
            // Также - взаимодействие вряд ли оптимально по объему переданных данных.

            // Шаг 0: проверка предусловий
            // Условие 1: url наследников - не повторяются.
            var countWithoutRepetitions = descendants.DistinctBy(d => d.Url).Count();
            if (descendants.Count > countWithoutRepetitions)
                throw new ArgumentException();

            // Шаг 1: Загрузка бранча, соответствующего поданной на вход ноде.
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var entityBranchParent = await context.CitilinkBranches.Include(b => b.Children).
                SingleAsync(b => b.Url == parent.DefaultLocation.Url);
            var childrenList = entityBranchParent.Children;

            // Шаг 2: присоединим к childrenList - нужных наследников.
            var addableChildrenList = descendants.ExceptBy(childrenList.Select(c => c.Url),
                st => st.Url).Select(CreateBranchFromTarget);
            childrenList.AddRange(addableChildrenList);
            await context.SaveChangesAsync();
        }

        public async Task<CitilinkNode?> GetUnprocessedNodeAsync(int maxErrorsCount = -1)
        {
            if (_retreivedNodeCache.All(nc => nc.AugmentationState
            == CitilinkNodeAugmentationState.Processed ||
            (nc.ProcessingErrorCount > maxErrorsCount && maxErrorsCount != -1)))
            {
                // берём до 50 элементов через 1 where? хардкод-константа.
                // maxErrorsCount - также хардкод-константа...
                using var context = await _dbContextFactory.CreateDbContextAsync();
                var branches = context.CitilinkBranches.Where(b => b.AugmentationState !=
                CitilinkNodeAugmentationState.Processed &&
                (b.ProcessingErrorCount <= maxErrorsCount || maxErrorsCount == -1)).Take(50);

                _retreivedNodeCache = branches.Select(CreateNodeFromBranch).ToList();
            }
            return _retreivedNodeCache.Find(cn => cn.AugmentationState !=
            CitilinkNodeAugmentationState.Processed &&
            (cn.ProcessingErrorCount <= maxErrorsCount || maxErrorsCount == -1));
        }


        public async Task RecreateTreeAsync()
        {
            // Переделать возможно посмотрев как работает удаление из БД в контексте
            // того что можно удалять когда что на что ссылается.

            // Также разобраться как сделать это в рамках возможно одного сетевого запроса к БД
            // (при возможно нескольких подзапросах в рамках... транзакции, чтоли)
            CitilinkCatalogBranchEntity rootDefault = new(mainCatalogUrl, false,
                CitilinkNodeAugmentationState.Unprocessed, CitilinkNodeType.RootCatalog);
            CitilinkCatalogUrlsTreeEntity treeEntity = new(rootDefault);
            CitilinkParsingExecutionStateEntity executionStateEntity = new(false,
                treeEntity);
            _logger?.LogTrace($"Tree's being recreated/created");

            using var context = await _dbContextFactory.CreateDbContextAsync();
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Удаляем полностью следы старого "древа".
                await context.CitilinkParsingExecutionStateEntity.ExecuteDeleteAsync();
                await context.Set<CitilinkCatalogUrlsTreeEntity>().ExecuteDeleteAsync();
                await context.CitilinkBranches.ExecuteDeleteAsync();

                // Создаем новое древо
                await context.CitilinkParsingExecutionStateEntity.AddAsync(executionStateEntity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка — база откатится к исходному состоянию, как будто метод и не вызывался
                await transaction.RollbackAsync();
                _logger?.LogError(ex, "Ошибка при пересоздании дерева каталогов. Изменения откатаны.");
                throw;
            }
        }


        /// <summary>
        /// Работает ТОЛЬКО с ненавигационными, собственными полями узла.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task UpdateNodeAsync(CitilinkNode node)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var branch = await context.CitilinkBranches.
                SingleAsync(b => b.Url == node.DefaultLocation.Url);
            UpdateBranchFieldsFromNode(branch, node);
            await context.SaveChangesAsync();
        }


        private void UpdateBranchFieldsFromNode(CitilinkCatalogBranchEntity updateable,
            CitilinkNode updater)
        {
            updateable.AugmentationState = updater.AugmentationState;
            updateable.DescendantsProcessed = updater.DescendantsProcessed;
            updateable.NodeType = updater.NodeType;
            updateable.ProcessingErrorCount = updater.ProcessingErrorCount;
        }
        private CitilinkNode CreateNodeFromBranch(CitilinkCatalogBranchEntity branch)
        {
            CitilinkScrapeTarget target = new(branch.Url, processed:
                branch.AugmentationState == CitilinkNodeAugmentationState.Processed);
            CitilinkNode node = new(target, branch.AugmentationState, branch.NodeType,
                branch.DescendantsProcessed, branch.ProcessingErrorCount);
            return node;
        }

        private CitilinkCatalogBranchEntity CreateBranchFromTarget(CitilinkScrapeTarget target)
        {
            return new(target.Url, false, CitilinkNodeAugmentationState.Unprocessed,
                CitilinkNodeType.Unknown);
        }
    }
}
