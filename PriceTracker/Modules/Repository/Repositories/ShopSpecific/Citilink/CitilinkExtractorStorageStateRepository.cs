using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Infrastructure;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkExtractorStorageStateRepository
    {

        private readonly PriceTrackerContext _dbContext;
        private readonly DbSet<CitilinkExtractorStorageStateEntity>
            _citilinkExtractorStorageState;

        public CitilinkExtractorStorageStateRepository(PriceTrackerContext dbContext)
        {

            _dbContext = dbContext;
            _citilinkExtractorStorageState = dbContext.CitilinkExtractionStorageStates;
            if (!_citilinkExtractorStorageState.Any())
            {
                _citilinkExtractorStorageState.Add(new(default, ""));
                _dbContext.SaveChanges();
            }
            else if (_citilinkExtractorStorageState.Count() > 1)
                throw new InvalidOperationException("Строк CitilinkExtractorStorageState" +
                    "в БД должно быть ровно 1.");

        }

        public void SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto)
        {
            var entity = _citilinkExtractorStorageState.Single();
            entity.StorageState = storageStateDto.StorageState;
            _dbContext.SaveChanges();
        }
        public CitilinkExtractorStorageStateDto GetExtractorStorageState()
        {
            var entity = _citilinkExtractorStorageState.Single();
            CitilinkExtractorStorageStateDto stateDto = new(entity.StorageState);
            return stateDto;
        }



    }
}
