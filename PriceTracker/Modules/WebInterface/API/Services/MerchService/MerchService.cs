using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;
using PriceTracker.Modules.WebInterface.API.Mapping.Merch;

namespace PriceTracker.Modules.WebInterface.API.Services.MerchService
{
    public class MerchService
    {
        private readonly IMerchRepositoryFacade _merchRepository;
        private readonly ICitilinkMerchRepositoryFacade _citilinkMerchRepository;

        private readonly IDetailedMerchDtoMapper _detailedMerchDtoMapper;
        public MerchService(IDetailedMerchDtoMapper detailedMerchDtoMapper,
            IMerchRepositoryFacade merchRepository,
            ICitilinkMerchRepositoryFacade citilinkMerchRepository)
        {
            _merchRepository = merchRepository;
            _citilinkMerchRepository = citilinkMerchRepository;
            _detailedMerchDtoMapper = detailedMerchDtoMapper;
        }
        public DetailedMerchDto? Get(int merchId)
        {
            var model = _merchRepository.GetModel(merchId);
            return model == null ? null : _detailedMerchDtoMapper.Map(model);
        }
        public DetailedMerchDto? GetCitilinkMerch(string citilinkMerchCode)
        {
            bool isGotten = _citilinkMerchRepository.TryGetSingleByCitilinkId
                (citilinkMerchCode, out var dto);

            if (!isGotten || dto == null)
                return null;
            else
                return _detailedMerchDtoMapper.Map(dto);
        }
    }
}
