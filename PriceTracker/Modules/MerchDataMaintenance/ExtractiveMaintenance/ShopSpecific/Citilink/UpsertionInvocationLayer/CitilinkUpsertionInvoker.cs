using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{
    public class CitilinkUpsertionInvoker : ICitilinkUpsertionInvoker
    {
        private readonly ICitilinkDataAugmenter _dataSynchronizer;
        private readonly ICitilinkCatalogTreeRepositoryAdapter _treeRepository;
        private readonly ICitilinkPolicyEnforcersContextfulProvider _enforcersProvider;

        // Логгер не внедрен пока-что... TODO!
        private readonly ILogger? _logger;
        private readonly SemaphoreSlim _semaphore;
        private Task? mainCycleTask;
        private bool cycleStopRequested;

        private const int msDelayBetweenRequests = 500;

        public CitilinkUpsertionInvoker(ICitilinkDataAugmenter dataSynchronizer,
            ICitilinkCatalogTreeRepositoryAdapter treeRepository,
            ICitilinkPolicyEnforcersContextfulProvider enforcersProvider,
            ILogger? logger)
        {
            _dataSynchronizer = dataSynchronizer;
            _treeRepository = treeRepository;
            _enforcersProvider = enforcersProvider;
            cycleStopRequested = false;
            _semaphore = new(1, 1);
            _logger = logger;
        }


        /// <summary>
        /// Что входит в частичный цикл? Будем считать предподготовку и заканчивание задачами основного цикла.
        /// </summary>
        /// <param name="maxRequests"></param>
        /// <returns></returns>
        public async Task<PartialCycleState> ExecutePartialCycleAsync
            (CitilinkEnforcersSharedContext cycleQuota, PartialCycleExecutionMode mode)
        {
            if (_semaphore.CurrentCount < 1)
            {
                throw new InvalidOperationException($"{nameof(ExecutePartialCycleAsync)}:\n" +
                    $" семафор занят: вероятно, была попытка запустить новый процесс " +
                    $"параллельно с другим. А это запрещено!");
            }
            await _semaphore.WaitAsync();
            var task = InnerlyExecutePartialCycleAsync(cycleQuota, mode);
            mainCycleTask = task;
            var result = await task;
            _semaphore.Release();
            return result;
        }

        // TODO: переименовать (innerly???)?
        private async Task<PartialCycleState> InnerlyExecutePartialCycleAsync
            (CitilinkEnforcersSharedContext cycleQuota, PartialCycleExecutionMode mode)
        {
            // TODO: разобраться с соответствием имени класса и объекта входного аргумента
            // (CitilinkEnforcersSharedContext)
            // в функцию - её назначению. Разрулить и разделить, или упростить? Или оба?
            CitilinkNode? node;

            switch (mode)
            {
                case PartialCycleExecutionMode.StartAnew:
                    await _treeRepository.RecreateTreeAsync();
                    break;
                case PartialCycleExecutionMode.Continue:
                    node = await _treeRepository.GetUnprocessedNodeAsync();
                    if (node == null)
                    {
                        return PartialCycleState.FullCompleted;
                    }
                    break;
                case PartialCycleExecutionMode.AutoChoice:
                    node = await _treeRepository.GetUnprocessedNodeAsync();
                    if (node == null)
                    {
                        await _treeRepository.RecreateTreeAsync();
                    }
                    break;
            }

            _enforcersProvider.SetActiveContext(cycleQuota);
            while (true)
            {
                // todo: можем ли мы так лихо обращаться к самому репозиторию отсюда (или его адаптеру) - без
                // прослоек без посредников?
                node = await _treeRepository.GetUnprocessedNodeAsync();
                _logger?.LogTrace($"{nameof(InnerlyExecutePartialCycleAsync)}: node {node} is taken");

                if (node == null)
                {
                    // Если GetAnyUnprocessed возвращает null ток когда кончаются необойденные узлы - это верно.
                    // todo: зафиксировать в юнит-тестах сие поведение? hmm...
                    return PartialCycleState.FullCompleted;
                }
                var nodeProcessingActions = await FullyProcessNodeAsync(node, 3);
                _logger?.LogTrace($"{nameof(InnerlyExecutePartialCycleAsync)}: node {node} " +
                    $"has PostHandlingAction as {nodeProcessingActions}");
                switch (nodeProcessingActions)
                {
                    case PostHandlingActions.None_Succcess:
                        continue;
                    case PostHandlingActions.RequestStop:
                        return PartialCycleState.StopRequested;
                    case PostHandlingActions.PartialCycle_ExpectedCooldown
                        or PostHandlingActions.PartialCycle_UnexpectedCooldown:
                        return PartialCycleState.PartialCompleted;
                    case PostHandlingActions.ConsiderNodeAugmentationError:
                        node.ProcessingErrorCount++;
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public async Task StopPartialCycleAsync()
        {
            cycleStopRequested = true;
            if (mainCycleTask != null && !mainCycleTask.IsCompleted)
            {
                await mainCycleTask;
            }
        }

        private async Task<PostHandlingActions> FullyProcessNodeAsync(CitilinkNode node,
            int maxPipelineRetriesOnErrors)
        {
            for (int i = 0; i < maxPipelineRetriesOnErrors; i++)
            {
                if (cycleStopRequested)
                    return PostHandlingActions.RequestStop;
                var postProcessingOrder = await AttemptToFullyProcessNode(node);
                switch (postProcessingOrder)
                {
                    case PostHandlingActions.None_Succcess:
                        return PostHandlingActions.None_Succcess;
                        break;

                    case PostHandlingActions.PartialCycle_ExpectedCooldown or
                    PostHandlingActions.PartialCycle_UnexpectedCooldown:
                        return postProcessingOrder;

                    case PostHandlingActions.RetryRequest:
                        continue;
                        break;

                    default:
                        throw new NotImplementedException();
                        break;
                }
            }
            return PostHandlingActions.ConsiderNodeAugmentationError;
        }

        private async Task<PostHandlingActions> AttemptToFullyProcessNode(CitilinkNode node)
        {
            if (cycleStopRequested)
                return PostHandlingActions.RequestStop;

            var enforcer = _enforcersProvider.ProvideForEntireNodeAugmentationEpisode();
            // этап 1: устанавливаем тип узла.
            if (node.NodeType == CitilinkNodeType.Unknown)
            {
                var state = await ProcessUntilTypeDiscovered(node);
                if (state != PostHandlingActions.None_Succcess)
                    return state;
            }

            if (cycleStopRequested)
                return PostHandlingActions.RequestStop;
            // этап 2: обрабатываем узел в зависимости от конкретного типа

            Task<PostHandlingActions> returnableTask;
            switch (node.NodeType)
            {

                case CitilinkNodeType.MerchCatalog:
                    returnableTask = CompleteProcessingMerchCatalogNode(node);
                    break;
                case CitilinkNodeType.RootCatalog or CitilinkNodeType.CatalogsCatalog:
                    returnableTask = CompleteProcessingRootOrCatalogsCatalogNode(node);
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
            var returnable = await returnableTask;
            if (returnable == PostHandlingActions.None_Succcess)
            {
                node.AugmentationState = CitilinkNodeAugmentationState.Processed;
                await _treeRepository.UpdateNodeAsync(node);
            }
            return returnable;
        }

        private async Task<PostHandlingActions> ProcessUntilTypeDiscovered(CitilinkNode node)
        {
            while (node.NodeType == CitilinkNodeType.Unknown)
            {
                var processingResult = await ProcessUnknownTypeNode(node);

                if (processingResult.errorsOccured)
                {
                    return processingResult.actions;
                }
            }
            return PostHandlingActions.None_Succcess;
        }


        private async Task<(PostHandlingActions actions, bool errorsOccured)>
            ProcessUnknownTypeNode(CitilinkNode node)
        {
            // Проверка входных данных на допустимость
            var type = node.NodeType;
            var state = node.AugmentationState;
            if (type != CitilinkNodeType.Unknown)
                throw new ArgumentException();
            if (state == CitilinkNodeAugmentationState.Processed
                || state == CitilinkNodeAugmentationState.PartiallyProcessed_TypeIsKnown)
                throw new InvalidOperationException();
            // Проверка входных данных на допустимость закончена

            if (cycleStopRequested)
                return (PostHandlingActions.RequestStop, false);
            var metaData = await AugmentNodeDataAsync(node, _enforcersProvider);
            if (StateIndicatesFailure(metaData.DataSyncMetadata.State))
            {
                return (HandleError(metaData), true);
            }
            else
                return (PostHandlingActions.None_Succcess, false);
        }

        private static bool StateIndicatesFailure(CitilinkNodeAugmenterAugmentationState state)
        {
            switch (state)
            {
                case CitilinkNodeAugmenterAugmentationState.Failure_PreviousLayer:
                    return true;
                default:
                    return false;
            }
        }



        private async Task<PostHandlingActions> CompleteProcessingMerchCatalogNode(CitilinkNode node)
        {
            var type = node.NodeType;
            var state = node.AugmentationState;
            if (type != CitilinkNodeType.MerchCatalog || state == CitilinkNodeAugmentationState.Processed
                || state == CitilinkNodeAugmentationState.Unprocessed)
                throw new ArgumentException();

            while (true)
            {
                if (node.LocationsList.Any(t => !t.IsProcessed))
                {
                    if (cycleStopRequested)
                        return PostHandlingActions.RequestStop;
                    var metadata = await AugmentNodeDataAsync(node, _enforcersProvider);
                    if (StateIndicatesFailure(metadata.DataSyncMetadata.State))
                    {
                        var actions = HandleError(metadata);
                        switch (actions)
                        {
                            case PostHandlingActions.None_Succcess:
                                break;

                            case PostHandlingActions.PartialCycle_ExpectedCooldown or
                            PostHandlingActions.PartialCycle_UnexpectedCooldown:
                                return actions;

                            case PostHandlingActions.RetryRequest:
                                continue;
                                break;
                        }
                        return actions;
                    }
                }
                else if (node.MerchNodeScrapeTargetsAugmentationState ==
                CitilinkMerchNodeScrapeTargetsAugmentationState.AllTargetsPresent)
                {
                    node.AugmentationState = CitilinkNodeAugmentationState.Processed;
                    return PostHandlingActions.None_Succcess;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private async Task<PostHandlingActions> CompleteProcessingRootOrCatalogsCatalogNode(CitilinkNode node)
        {
            var type = node.NodeType;
            var state = node.AugmentationState;
            if ((type != CitilinkNodeType.CatalogsCatalog && type != CitilinkNodeType.RootCatalog)
                || state == CitilinkNodeAugmentationState.Processed
                || state == CitilinkNodeAugmentationState.Unprocessed)
                throw new ArgumentException();

            while (node.LocationsList.Any(st => !st.IsProcessed))
            {
                if (cycleStopRequested)
                    return PostHandlingActions.RequestStop;
                var metadata = await AugmentNodeDataAsync(node, _enforcersProvider);

                if (StateIndicatesFailure(metadata.DataSyncMetadata.State))
                {
                    // TODO: DRY violation with actions = , switch (actions)
                    var actions = HandleError(metadata);
                    switch (actions)
                    {
                        case PostHandlingActions.None_Succcess:
                            break;

                        case PostHandlingActions.PartialCycle_ExpectedCooldown or
                        PostHandlingActions.PartialCycle_UnexpectedCooldown:
                            return actions;

                        case PostHandlingActions.RetryRequest:
                            continue;
                            break;
                    }
                }
            }

            node.AugmentationState = CitilinkNodeAugmentationState.Processed;
            return PostHandlingActions.None_Succcess;
        }



        protected enum PostHandlingActions
        {
            None_Succcess, PartialCycle_ExpectedCooldown,
            PartialCycle_UnexpectedCooldown, RetryRequest, ConsiderNodeAugmentationError,
            RequestStop
        };
        // Make it so it would be returning commands on further actions? Or not...
        private PostHandlingActions HandleError(CitilinkExecutionMetadata metadata)
        {
            var dataSyncStatus = metadata.DataSyncMetadata.State;
            var parsingStatus = metadata.ParseMetadata.State;
            var scrapingStatus = metadata.ScrapeMetadata.CompletionState;

            switch (dataSyncStatus, parsingStatus, scrapingStatus)
            {
                case (CitilinkNodeAugmenterAugmentationState.Success, _, _):
                    throw new InvalidOperationException();

                case (CitilinkNodeAugmenterAugmentationState.Failure_PreviousLayer,
                    ParseCompletionState.Failure_PreviousLayer, _):
                    return HandleScrapingError(metadata.ScrapeMetadata);
                    break;

                case (CitilinkNodeAugmenterAugmentationState.Failure_PreviousLayer, _, _):
                    return HandleParsingError(metadata.ParseMetadata);
                    break;

                default:
                    return HandleDataSyncError(metadata.DataSyncMetadata);
                    break;
            }
        }
        private PostHandlingActions HandleDataSyncError(CitilinkNodeAugmentMetadata metadata)
        {
            throw new NotImplementedException("Когда писали метод - ошибок в Augment слое не было");
        }
        private PostHandlingActions HandleParsingError(CitilinkParseMetadata metadata)
        {
            switch (metadata.State)
            {
                case ParseCompletionState.Success or
                    ParseCompletionState.Failure_PreviousLayer:
                    throw new ArgumentException();

                case ParseCompletionState.Unsuccess_UnknownParsable:
                    _logger?.LogWarning($"На вход парсера было подано что-то неожиданное.");
                    return PostHandlingActions.RetryRequest;
                    break;

                default:
                    // Точно NotImplemented нужно в неучтенных кейсах?
                    throw new NotImplementedException();
                    break;
            }
        }
        private PostHandlingActions HandleScrapingError(CitilinkScrapeMetadata metadata)
        {
            switch (metadata.CompletionState)
            {
                case ScrapeCompletionState.Success:
                    throw new ArgumentException();
                    break;

                case ScrapeCompletionState.StoppedAsPolicyBudgetsReached:
                    _logger?.LogInformation($"Процесс подлежит приостановке: исчерпаны лимиты политик.");
                    return PostHandlingActions.PartialCycle_ExpectedCooldown;
                    break;

                case ScrapeCompletionState.UnallowedStatus:
                    _logger?.LogWarning($"Вернулся неожиданный StatusCode. Программа " +
                        $"повторит запрос.");
                    return PostHandlingActions.RetryRequest;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<CitilinkExecutionMetadata> AugmentNodeDataAsync(CitilinkNode node,
            ICitilinkPolicyEnforcersContextfulProvider provider)
        {
            CitilinkExecutionMetadata metadata;

            var enforcer = provider.ProvideForEntireNodeAugmentationEpisode();

            enforcer.TryRegisterScrapeRequest();
            await Task.Delay(msDelayBetweenRequests);
            metadata = await _dataSynchronizer.AugmentNodeDataAsync(node, enforcer);
            if (metadata.DataSyncMetadata.State == CitilinkNodeAugmenterAugmentationState.Success)
                return metadata;
            while (enforcer.TryRegisterScrapeRequest())
            {
                metadata = await _dataSynchronizer.AugmentNodeDataAsync(node, enforcer);
                if (metadata.DataSyncMetadata.State == CitilinkNodeAugmenterAugmentationState.Success)
                    return metadata;
            }
            return metadata;
        }
    }
}
