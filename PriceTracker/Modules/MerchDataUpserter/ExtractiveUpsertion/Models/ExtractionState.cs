namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models
{
    public abstract class ExtractionState(bool isCompleted)
    {

        /// <summary>
        /// Переменная указывает, был ли завершен процесс.
        /// Если процесс был завершен - логично начать его сначала.
        /// Если нет - возобновить с места остановки.
        /// </summary>
        public bool IsCompleted = isCompleted;
    }
}
