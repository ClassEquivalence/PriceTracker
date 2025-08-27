namespace PriceTracker.Core.Models.Process
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
