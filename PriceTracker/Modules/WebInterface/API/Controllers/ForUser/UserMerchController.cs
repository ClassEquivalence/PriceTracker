using Microsoft.AspNetCore.Mvc;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Routing;
using PriceTracker.Modules.WebInterface.API.Services.MerchService;

namespace PriceTracker.Modules.WebInterface.API.Controllers.ForUser
{
    [Route(ControllerRoutes.UserMerchControllerRoute)]
    [ApiController]
    public class UserMerchController : ControllerBase
    {

        private readonly MerchService _merchService;

        private readonly ILogger _logger;


        public UserMerchController(ILogger<Program> logger,
            IWebInterfaceMapperProvider mapperProvider,
            IMerchRepositoryFacade merchRepository,
            ITimestampedPriceRepositoryFacade timestampedPriceRepository,
            IPriceHistoryRepositoryFacade priceHistoryRepository,
            IShopRepositoryFacade shopRepository,
            ICitilinkMerchRepositoryFacade citilinkMerchRepository)
        {
            _logger = logger;
            _merchService = new(logger, mapperProvider.DetailedMerchDtoMapper,
                mapperProvider.OverviewMerchDtoMapper,
                merchRepository, timestampedPriceRepository,
                priceHistoryRepository, shopRepository, citilinkMerchRepository);
        }

        [HttpGet("{merchId:int}")]
        public DetailedMerchDto? Get(int merchId)
        {
            return _merchService.Get(merchId);
        }

        [HttpGet("citilink/{citilinkMerchCode}")]
        public DetailedMerchDto? GetCitilinkMerch(string citilinkMerchCode)
        {
            return _merchService.GetCitilinkMerch(citilinkMerchCode);
        }

    }
}
