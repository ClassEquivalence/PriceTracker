using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Process;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class ProcessExtractionTimeRepository : IProcessExtractionTimeRepository
    {
        private readonly LastTimeExtractionHappenedRepository _timeExtractionHappenedRepository;
        public ProcessExtractionTimeRepository(LastTimeExtractionHappenedRepository
            repository)
        {
            _timeExtractionHappenedRepository = repository;
        }

        public (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened()
        {
            return _timeExtractionHappenedRepository.GetLastTimeExtractionProcessHappened();
        }

        public void SetFinishTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetFinishTimeExtractionProcessHappened(time);
        }

        public void SetStartTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetStartTimeExtractionProcessHappened(time);
        }
    }
}
