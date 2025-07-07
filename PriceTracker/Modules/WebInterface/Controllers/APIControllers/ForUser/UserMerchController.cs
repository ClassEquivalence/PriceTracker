using Microsoft.AspNetCore.Mvc;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;
using PriceTracker.Modules.WebInterface.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.Routing;
using PriceTracker.Modules.WebInterface.Services.MerchService;

namespace PriceTracker.Modules.WebInterface.Controllers.APIControllers.ForUser
{
    [Route(ControllerRoutes.UserMerchControllerRoute)]
    [ApiController]
    public class UserMerchController : ControllerBase
    {

        private readonly MerchService _merchService;

        private readonly ILogger _logger;


        public UserMerchController(ILogger<Program> logger,
            IWebInterfaceMapperProvider mapperProvider, IRepositoryFacade repositoryFacade)
        {
            _logger = logger;
            _merchService = new(logger, repositoryFacade, mapperProvider.DetailedMerchDtoMapper,
                mapperProvider.OverviewMerchDtoMapper);
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
