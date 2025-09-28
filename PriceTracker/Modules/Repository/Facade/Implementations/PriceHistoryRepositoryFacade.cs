using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class PriceHistoryRepositoryFacade : IPriceHistoryRepositoryFacade
    {
        private readonly PriceHistoryRepository _priceHistoryRepository;
        public PriceHistoryRepositoryFacade(PriceHistoryRepository repository)
        {
            _priceHistoryRepository = repository;
        }

        public bool Any(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.Any(predicate);
        }

        public void Create(MerchPriceHistoryDto entity)
        {
            _priceHistoryRepository.Create(entity);
        }

        public bool Delete(int id)
        {
            return _priceHistoryRepository.Delete(id);
        }

        public List<MerchPriceHistoryDto> GetAll()
        {
            return _priceHistoryRepository.GetAll();
        }

        public MerchPriceHistoryDto? GetModel(int id)
        {
            return _priceHistoryRepository.GetModel(id);
        }

        public MerchPriceHistoryDto? SingleOrDefault(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.SingleOrDefault(predicate);
        }

        public bool Update(MerchPriceHistoryDto entity)
        {
            return _priceHistoryRepository.Update(entity);
        }

        public List<MerchPriceHistoryDto> Where(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.Where(predicate);
        }
    }
}
