using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Routing;
using PriceTracker.Modules.WebInterface.API.Services.MerchService;

using PriceTracker.Modules.WebInterface.API.Filters;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// TODO: СДЕЛАТЬ НОРМАЛЬНУЮ МАРШРУТИЗАЦИЮ!
// TODO: В Ok(), наверное, следует возвращать созданный объект. Проверить на соответствие REST.

namespace PriceTracker.Modules.WebInterface.API.Controllers.ForAdmin
{
    [Route(ControllerRoutes.AdminMerchControllerRoute)]
    [ApiController]
    [AdminAPIAuthorization]
    public class AdminMerchController : ControllerBase
    {

        private readonly MerchService _merchService;

        private readonly ILogger _logger;


        public AdminMerchController(ILogger<Program> logger,
            IWebInterfaceMapperProvider mapperProvider, IRepositoryFacade repositoryFacade)
        {
            _logger = logger;
            _merchService = new(logger, repositoryFacade, mapperProvider.DetailedMerchDtoMapper,
                mapperProvider.OverviewMerchDtoMapper);
        }


        [HttpGet("shop/{shopId:int}")]
        public IEnumerable<MerchOverviewDto> GetMerchesOfShop(int shopId)
        {
            return _merchService.GetMerchesOfShop(shopId);
        }


        [HttpGet("{merchId:int}")]
        public DetailedMerchDto? Get(int merchId)
        {
            return _merchService.Get(merchId);
        }


        [HttpPost("{shopId:int}")]
        public IActionResult Post(int shopId, MerchOverviewDto merch)
        {

            bool isCreated = _merchService.Post(shopId, merch);
            return isCreated ? Created() :
                StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpPut("{merchId:int}")]
        public IActionResult Put(int merchId, string name)
        {
            bool isChanged = _merchService.Put(merchId, name);
            return isChanged ? Ok() : NotFound();
        }


        [HttpDelete("{merchId:int}")]
        public IActionResult Delete(int merchId)
        {
            if (_merchService.Delete(merchId))
                return Ok();
            else
                return NotFound();
        }

        [HttpPost("{merchId:int}/price")]
        public IActionResult PostPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {
            bool isAdded = _merchService.PostPrice(merchId, timestampedPrice);

            if (isAdded)
                return Ok();
            else
                return NotFound();
        }


        [HttpPost("{merchId:int}/price/current")]
        public IActionResult SetCurrentPrice(int merchId, decimal currentPrice)
        {
            bool isPriceSet = _merchService.SetCurrentPrice(merchId, currentPrice);
            if (isPriceSet)
                return Ok();
            else
                return Conflict();
        }

        [HttpDelete("price/{timestampedPriceId:int}")]
        public IActionResult RemoveTimestampedPrice(int timestampedPriceId)
        {
            bool isRemoved = _merchService.RemoveTimestampedPrice(timestampedPriceId);
            if (isRemoved)
                return Ok();
            else
                return NotFound();
        }

        [HttpDelete("{merchId:int}/price")]
        public IActionResult ClearOldPrices(int merchId)
        {
            bool isRemoved = _merchService.ClearOldPrices(merchId);
            if (isRemoved)
                return Ok();
            else
                return Conflict();
        }


        [HttpGet("citilink/{citilinkMerchCode}")]
        public DetailedMerchDto? GetCitilinkMerch(string citilinkMerchCode)
        {
            return _merchService.GetCitilinkMerch(citilinkMerchCode);
        }

    }
}
