namespace PriceTracker.Core.CommonModels.Process
{
    public record ExtractionStateDto : BaseDto
    {
        public bool IsCompleted { get; set; }
        public ExtractionStateDto(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }
    }
}
