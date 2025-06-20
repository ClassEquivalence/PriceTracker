namespace PriceTracker.Modules.Repository.Entities.Process
{
    public class TimeExtractionProcessHappened
    {
        public TimeExtractionProcessHappened(DateTime LastTimeStarted,
            DateTime LastTimeFinished)
        {
            this.LastTimeFinished = LastTimeFinished;
            this.LastTimeStarted = LastTimeStarted;
        }
        public DateTime LastTimeStarted { get; set; }
        public DateTime LastTimeFinished { get; set;}
    }
}
