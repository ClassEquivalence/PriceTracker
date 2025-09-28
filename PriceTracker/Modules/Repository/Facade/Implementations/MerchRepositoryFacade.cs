using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class MerchRepositoryFacade : IMerchRepositoryFacade
    {

        private readonly MerchRepository _merchRepository;

        public MerchRepositoryFacade(MerchRepository repository)
        {
            _merchRepository = repository;
        }

        public bool Any(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.Any(predicate);
        }

        public void Create(MerchDto entity)
        {
            _merchRepository.Create(entity);
        }

        public bool Delete(int id)
        {
            return _merchRepository.Delete(id);
        }

        public List<MerchDto> GetAll()
        {
            return _merchRepository.GetAll();
        }

        public MerchDto? GetModel(int id)
        {
            return _merchRepository.GetModel(id);
        }

        public MerchDto? SingleOrDefault(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.SingleOrDefault(predicate);
        }

        public bool Update(MerchDto entity)
        {
            return _merchRepository.Update(entity);
        }

        public List<MerchDto> Where(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.Where(predicate);
        }
    }
}
