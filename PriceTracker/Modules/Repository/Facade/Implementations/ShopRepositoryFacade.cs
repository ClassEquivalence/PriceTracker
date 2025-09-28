using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class ShopRepositoryFacade : IShopRepositoryFacade
    {
        public ShopRepository _shopRepository;
        public ShopRepositoryFacade(ShopRepository repository)
        {
            _shopRepository = repository;
        }

        public bool Any(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.Any(predicate);
        }

        public void Create(ShopDto dto)
        {
            _shopRepository.Create(dto);
        }

        public bool Delete(int id)
        {
            return _shopRepository.Delete(id);
        }

        public List<ShopDto> GetAll()
        {
            return _shopRepository.GetAll();
        }

        public ShopDto? GetModel(int id)
        {
            return _shopRepository.GetModel(id);
        }

        public ShopDto? SingleOrDefault(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.SingleOrDefault(predicate);
        }

        public bool Update(ShopDto entity)
        {
            return _shopRepository.Update(entity);
        }

        public List<ShopDto> Where(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.Where(predicate);
        }
    }
}
