using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine;
using PriceTracker.Modules.Repository.Facade;

namespace PriceTracker.Modules.MerchDataProvider.Upsertion.ScheduledTriggerer
{
    public class ScheduledMerchUpserter
    {

        private readonly IRepositoryFacade _repository;

        // TODO: Реализовать получение этого поля из внешнего источника.
        //Да и в целом поле реализовать.
        private DateTime _lastTimeExtractionStarted;
        private DateTime _lastTimeExtractionFinished;
        private bool _previousExtractionFinished
        {
            get
            {
                return _lastTimeExtractionFinished > _lastTimeExtractionStarted ?
                    true : false;
            }
        }
        private bool _extractionTimeoutExpired
        {
            get
            {
                return DateTime.Now > _lastTimeExtractionStarted +
                    Configs.PriceUpdatePeriod;
            }
        }
        private TimeSpan _timeBeforeExtraction => _lastTimeExtractionStarted
            + Configs.PriceUpdatePeriod - DateTime.Now;

        private readonly MerchExtractionCoordinator _coordinator;
        private readonly ILogger? _logger;
        public ScheduledMerchUpserter(MerchExtractionCoordinator coordinator,
            (DateTime lastTimeExtractionStarted, DateTime lastTimeExtractionFinished)
            extractionTimingData, IRepositoryFacade repository, ILogger? logger = null)
        {
            _coordinator = coordinator;
            _lastTimeExtractionFinished = extractionTimingData.lastTimeExtractionFinished;
            _lastTimeExtractionStarted = extractionTimingData.lastTimeExtractionStarted;
            _repository = repository;
            _logger = logger;
        }

        // TODO: контроль за временем.
        private async Task OnStartRestart()
        {
            _logger?.LogTrace($"{nameof(ScheduledMerchUpserter)}: OnStartRestart метод запущен.");
            if (_previousExtractionFinished && _extractionTimeoutExpired)
            {
                _logger?.LogTrace($"{nameof(ScheduledMerchUpserter)}: запущен новый сеанс извлечения" +
                    $"товаров.");
                await StartNewExtraction();
            }
            else if (!_previousExtractionFinished)
            {
                _logger?.LogTrace($"{nameof(ScheduledMerchUpserter)}: продолжается предыдущий сеанс" +
                    $"извлечения товаров.");
                await ContinuePreviousExtraction();
            }
            else if (_previousExtractionFinished && !_extractionTimeoutExpired)
            {
                // do nothing.
                _logger?.LogTrace($"{nameof(ScheduledMerchUpserter)}: Извлечение товаров пока не" +
                    $"осуществляется.");
            }
        }
        private async Task LaunchRepeatedExtraction()
        {
            _logger?.LogTrace($"{nameof(ScheduledMerchUpserter)}: Запущен {nameof(LaunchRepeatedExtraction)}.");
            while (true)
            {
                if (_extractionTimeoutExpired)
                    await StartNewExtraction();
                else
                {
                    if (_timeBeforeExtraction.TotalMilliseconds > 0)
                        await Task.Delay(_timeBeforeExtraction);
                    else
                        continue;
                }
            }
        }

        private async Task StartNewExtraction()
        {
            _lastTimeExtractionStarted = DateTime.Now;
            // TODO: Добавить общение координатора с 
            // этим классом посредством выброса исключения
            // в экстракторе. Иначе время может работать некорректно.
            _repository.SetStartTimeExtractionProcessHappened(_lastTimeExtractionStarted);

            await _coordinator.StartNewExtraction();
            _lastTimeExtractionFinished = DateTime.Now;
            _repository.SetFinishTimeExtractionProcessHappened(_lastTimeExtractionFinished);
        }

        private async Task ContinuePreviousExtraction()
        {
            // TODO: Добавить общение координатора с 
            // этим классом посредством выброса исключения
            // в экстракторе. Иначе время может работать некорректно.
            await _coordinator.ContinuePreviousExtraction();
            _lastTimeExtractionFinished = DateTime.Now;
            _repository.SetFinishTimeExtractionProcessHappened(_lastTimeExtractionFinished);
        }

        public async Task ProcessScheduledExtraction()
        {
            await OnStartRestart();
            await LaunchRepeatedExtraction();
        }

    }
}
