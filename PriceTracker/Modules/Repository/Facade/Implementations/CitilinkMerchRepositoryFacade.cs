using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class CitilinkMerchRepositoryFacade : ICitilinkMerchRepositoryFacade
    {

        private readonly CitilinkMerchRepository _citilinkMerchRepository;
        private readonly ILogger _logger;

        public CitilinkMerchRepositoryFacade(CitilinkMerchRepository merchRepository,
            ILogger<CitilinkMerchRepositoryFacade> logger)
        {
            _citilinkMerchRepository = merchRepository;
            _logger = logger;
        }

        public bool TryGetSingleByCitilinkId(string citilinkId, out CitilinkMerchDto? dto)
        {
            dto = _citilinkMerchRepository.SingleOrDefault(cm => cm.CitilinkId == citilinkId);
            return !(dto == null);
        }

        public bool TryInsert(CitilinkMerchDto dto)
        {
            _citilinkMerchRepository.Create(dto);
            return true;
        }

        public bool TryUpdate(CitilinkMerchDto dto)
        {
            return _citilinkMerchRepository.Update(dto);
        }

        public async Task CreateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchRepositoryFacade)}, " +
                $"{nameof(CreateManyAsync)}: метод вызван.");
            await _citilinkMerchRepository.CreateManyAsync(citilinkMerches);
        }

        public List<CitilinkMerchDto> GetMultiple(string[] citilinkIds)
        {
            return _citilinkMerchRepository.Where(cm => citilinkIds.Contains(cm.CitilinkId)).ToList();
        }

        public async Task UpdateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchRepositoryFacade)}, " +
                $"{nameof(UpdateManyAsync)}: метод вызван.");
            await _citilinkMerchRepository.UpdateManyAsync(citilinkMerches);
        }

        public List<CitilinkMerchDto> Where(Func<CitilinkMerchDto, bool> selector)
        {
            return _citilinkMerchRepository.Where(selector);
        }
    }
}
