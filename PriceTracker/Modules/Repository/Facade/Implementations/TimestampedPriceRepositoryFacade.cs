using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class TimestampedPriceRepositoryFacade : ITimestampedPriceRepositoryFacade
    {
        private readonly TimestampedPriceRepository _timestampedPriceRepository;
        public TimestampedPriceRepositoryFacade(TimestampedPriceRepository repository)
        {
            _timestampedPriceRepository = repository;
        }

        public bool Any(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.Any(predicate);
        }

        public void Create(TimestampedPriceDto entity)
        {
            _timestampedPriceRepository.Create(entity);
        }

        public bool Delete(int id)
        {
            return _timestampedPriceRepository.Delete(id);
        }

        public List<TimestampedPriceDto> GetAll()
        {
            return _timestampedPriceRepository.GetAll();
        }

        public TimestampedPriceDto? GetModel(int id)
        {
            return _timestampedPriceRepository.GetModel(id);
        }

        public TimestampedPriceDto? SingleOrDefault(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.SingleOrDefault(predicate);
        }

        public bool Update(TimestampedPriceDto entity)
        {
            return _timestampedPriceRepository.Update(entity);
        }

        public List<TimestampedPriceDto> Where(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.Where(predicate);
        }
    }
}
