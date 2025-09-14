using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchRepository : MerchSubtypeRepository<CitilinkMerchDto,
        CitilinkMerchEntity, PriceTrackerContext>
    {
        public CitilinkMerchRepository(CitilinkMerchEntityRepository entityRepository,
            ICoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>
            mapper, ILogger? logger = null) : base(entityRepository, mapper, logger)
        {
        }

        public override async Task CreateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchRepository)}, {nameof(CreateManyAsync)}: метод вызван.");
            await ((CitilinkMerchEntityRepository)EntityRepository).CreateManyAsync(citilinkMerches
                .Select(Mapper.Map).ToList());
        }

        public async Task UpdateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchRepository)}, {nameof(UpdateManyAsync)}: метод вызван.");
            await ((CitilinkMerchEntityRepository)EntityRepository).UpdateManyAsync(citilinkMerches
                .Select(Mapper.Map).ToList());
        }

    }
}
