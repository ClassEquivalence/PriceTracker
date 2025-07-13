namespace PriceTracker.Modules.Repository.Entities.Infrastructure
{
    public class CitilinkExtractorStorageStateEntity :
        BaseEntity
    {
        public string StorageState { get; set; }
        public CitilinkExtractorStorageStateEntity(int Id,
            string storageState) : base(Id)
        {
            StorageState = storageState;
        }
    }
}
