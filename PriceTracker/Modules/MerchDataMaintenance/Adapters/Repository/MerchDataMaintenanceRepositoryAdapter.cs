using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.CommonModels.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public class MerchDataMaintenanceRepositoryAdapter : IMerchDataMaintenanceRepositoryAdapter
    {
        IDbContextFactory<PriceTrackerContext> _dbContextFactory;
        ICitilinkMerchCoreToEntityMapper _mapper;
        public MerchDataMaintenanceRepositoryAdapter(IDbContextFactory<PriceTrackerContext>
            dbContextFactory, ICitilinkMerchCoreToEntityMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
        }


        public async Task CreateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            // Выбрасывать в случае чего исключения? поделать как то пайплайн работы по своему?
            await context.CitilinkMerches.AddRangeAsync(citilinkMerches.Select(_mapper.Map));
            await context.SaveChangesAsync();
        }

        public List<CitilinkMerchDto> GetMultiple(string[] citilinkIds)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var merchEntities = context.CitilinkMerches.AsNoTracking()
                .Where(m => citilinkIds.Contains(m.CitilinkId))
                .Include(m => m.PriceHistory)
                    .ThenInclude(ph => ph.TimestampedPrices)
                .Include(m => m.PriceHistory)
                    .ThenInclude(ph => ph.CurrentPricePointer).ThenInclude(cpp => cpp.CurrentPrice);
            return merchEntities.Select(_mapper.Map).ToList();
        }

        public async Task UpdateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            var contextTask = _dbContextFactory.CreateDbContextAsync();
            var merchEntities = citilinkMerches.Select(_mapper.Map);
            using var context = await contextTask;
            context.CitilinkMerches.UpdateRange(merchEntities);
            await context.SaveChangesAsync();
        }
    }
}
