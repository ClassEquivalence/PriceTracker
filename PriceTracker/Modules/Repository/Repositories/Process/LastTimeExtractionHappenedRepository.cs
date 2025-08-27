using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Process;
using PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository;

namespace PriceTracker.Modules.Repository.Repositories.Process
{


    // TODO: Сделано костыльно: не наследуется от общих классов/интерфейсов. Можно и переделать.
    public class LastTimeExtractionHappenedRepository
    {
        private readonly PriceTrackerContext _dbContext;
        private readonly DbSet<TimeExtractionProcessHappened> _timeProcessHappened;
        public LastTimeExtractionHappenedRepository(PriceTrackerContext dbContext)
        {
            _dbContext = dbContext;
            _timeProcessHappened = dbContext.TimeExtractionProcessHappened;
            if (!_timeProcessHappened.Any())
            {
                _timeProcessHappened.Add(new(DateTime.MinValue, DateTime.MinValue));
                _dbContext.SaveChanges();
            }
            else if (_timeProcessHappened.Count() > 1)
                throw new InvalidOperationException("Строк TimeExtractionProcessHappened" +
                    "в БД должно быть ровно 1.");

        }

        public (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened()
        {
            var timeExtracted = _timeProcessHappened.Single();
            var finished = timeExtracted.LastTimeFinished;
            var started = timeExtracted.LastTimeStarted;

            return (started, finished);
        }

        public void SetFinishTimeExtractionProcessHappened(DateTime time)
        {
            var timeExtracted = _timeProcessHappened.Single();
            timeExtracted.LastTimeFinished = time;
            _dbContext.SaveChanges();
        }

        public void SetStartTimeExtractionProcessHappened(DateTime time)
        {
            var timeExtracted = _timeProcessHappened.Single();
            timeExtracted.LastTimeStarted = time;
            _dbContext.SaveChanges();
        }
    }
}
