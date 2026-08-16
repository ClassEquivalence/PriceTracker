using Microsoft.AspNetCore.Mvc;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.Adapters.Repository;
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
        private readonly IMerchRepositoryAdapter _merchRepository;

        private readonly ILogger _logger;


        public UserMerchController(ILogger<Program> logger,
            IMerchRepositoryAdapter merchRepositoryAdapter)
        {
            _logger = logger;
            _merchRepository = merchRepositoryAdapter;
        }

        [HttpGet("{merchId:int}")]
        public async Task<DetailedMerchDto?> Get(int merchId)
        {
            return await _merchRepository.GetMerchAsync(merchId);
        }
        [HttpGet("citilink/{citilinkMerchCode}")]
        public async Task<DetailedMerchDto?> GetCitilinkMerch(string citilinkMerchCode)
        {
            return await _merchRepository.GetCitilinkMerchAsync(citilinkMerchCode);
        }

    }
}
