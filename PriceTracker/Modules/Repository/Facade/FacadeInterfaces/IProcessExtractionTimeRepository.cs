namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IProcessExtractionTimeRepository
    {
        (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened();

        void SetStartTimeExtractionProcessHappened(DateTime time);
        void SetFinishTimeExtractionProcessHappened(DateTime time);
    }
}
