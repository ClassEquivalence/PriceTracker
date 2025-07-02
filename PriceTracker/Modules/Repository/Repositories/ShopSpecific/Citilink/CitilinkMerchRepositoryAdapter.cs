using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchRepositoryAdapter : IMerchSubtypeRepositoryAdapter
    {
        private readonly CitilinkMerchRepository _actualRepository;
        public CitilinkMerchRepositoryAdapter(CitilinkMerchRepository citilinkMerchRepository)
        {
            _actualRepository = citilinkMerchRepository;

        }
        public Type HandledType { get => typeof(CitilinkMerchDto); }

        public bool Any(Func<MerchDto, bool> predicate)
        {
            return _actualRepository.Any(predicate);
        }

        public void Create(MerchDto entity)
        {
            var citilinkMerchDto = entity as CitilinkMerchDto;
            if (citilinkMerchDto != null)
                _actualRepository.Create(citilinkMerchDto);
            else
                throw new InvalidOperationException("Не следует создавать через " +
                    "CitilinkMerchRepository товары другого типа");
        }

        public bool Delete(int id)
        {
            return _actualRepository.Delete(id);
        }

        public void SaveChanges()
        {
            _actualRepository.SaveChanges();
        }

        public MerchDto? SingleOrDefault(Func<MerchDto, bool> predicate)
        {
            return _actualRepository.SingleOrDefault(predicate);
        }

        public bool Update(MerchDto entity)
        {
            var citilinkMerchDto = entity as CitilinkMerchDto;
            if (citilinkMerchDto != null)
                return _actualRepository.Update(citilinkMerchDto);
            else
                throw new InvalidOperationException("Не следует обновлять через " +
                    "CitilinkMerchRepository товары другого типа");
        }

        public List<MerchDto> Where(Func<MerchDto, bool> predicate)
        {
            return _actualRepository.Where(predicate).Select(CitilinkMerchDtoToMerchDto)
                .ToList();
        }

        private static MerchDto CitilinkMerchDtoToMerchDto(CitilinkMerchDto model)
        {
            return new MerchDto(model.Id, model.Name, model.PriceTrack, model.ShopId, model.PriceHistoryId);
        }
    }
}
