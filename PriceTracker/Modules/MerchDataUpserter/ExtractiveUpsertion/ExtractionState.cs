namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion
{
    public abstract class ExtractionState(bool isCompleted,
        int id = default)
    {
        public int Id = id;

        /// <summary>
        /// Переменная указывает, был ли завершен процесс.
        /// Если процесс был завершен - логично начать его сначала.
        /// Если нет - возобновить с места остановки.
        /// </summary>
        public bool IsCompleted = isCompleted;
    }
}
