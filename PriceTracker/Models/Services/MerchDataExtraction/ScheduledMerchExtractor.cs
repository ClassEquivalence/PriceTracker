namespace PriceTracker.Models.Services.MerchDataExtraction
{
    public class ScheduledMerchExtractor
    {

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
        public ScheduledMerchExtractor(MerchExtractionCoordinator coordinator,
            (DateTime lastTimeExtractionStarted, DateTime lastTimeExtractionFinished)
            extractionTimingData)
        {
            _coordinator = coordinator;
            _lastTimeExtractionFinished = extractionTimingData.lastTimeExtractionFinished;
            _lastTimeExtractionStarted = extractionTimingData.lastTimeExtractionStarted;
        }

        // TODO: контроль за временем.
        private async Task OnStartRestart()
        {
            if (_previousExtractionFinished && _extractionTimeoutExpired)
            {
                await StartNewExtraction();
            }
            else if (!_previousExtractionFinished)
            {
                await ContinuePreviousExtraction();
            }
            else if(_previousExtractionFinished && !_extractionTimeoutExpired)
            {
                // do nothing.
            }
        }
        private async Task LaunchRepeatedExtraction()
        {
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
            await _coordinator.StartNewExtraction();
            _lastTimeExtractionFinished = DateTime.Now;
        }

        private async Task ContinuePreviousExtraction()
        {
            // TODO: Добавить общение координатора с 
            // этим классом посредством выброса исключения
            // в экстракторе. Иначе время может работать некорректно.
            await _coordinator.ContinuePreviousExtraction();
            _lastTimeExtractionFinished = DateTime.Now;
        }

        public async Task ProcessScheduledExtraction()
        {
            await OnStartRestart();
            await LaunchRepeatedExtraction();
        }

    }
}
