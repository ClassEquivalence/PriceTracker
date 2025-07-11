namespace PriceTracker.Modules.Repository.Facade.Citilink
{
    public interface ICitilinkMiscellaneousRepositoryFacade
    {
        public void SetExtractorStorageState(string storageState);
        public string GetExtractorStorageState();
    }
}
