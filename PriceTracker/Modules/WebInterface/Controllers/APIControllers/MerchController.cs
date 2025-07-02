using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;
using PriceTracker.Modules.WebInterface.Services.InterfaceServices;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// TODO: СДЕЛАТЬ НОРМАЛЬНУЮ МАРШРУТИЗАЦИЮ!
// TODO: В Ok(), наверное, следует возвращать созданный объект. Проверить на соответствие REST.

namespace PriceTracker.Modules.WebInterface.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchController : ControllerBase
    {

        private readonly AdminAPIService _adminApiService;

        private readonly ILogger _logger;


        public MerchController(ILogger<Program> logger, AdminAPIService service)
        {
            _logger = logger;
            _adminApiService = service;
        }

        // GET: api/<MerchController>
        [HttpGet("shop/{shopId:int}")]
        public IEnumerable<MerchOverviewDto> GetMerchesOfShop(int shopId)
        {
            return _adminApiService.GetMerchesOfShop(shopId);
        }

        // GET api/<MerchController>/1/3
        [HttpGet("{merchId:int}")]
        public DetailedMerchDto? Get(int merchId)
        {
            return _adminApiService.GetDetailedMerch(merchId);
        }

        // POST api/<MerchController>/1
        [HttpPost("{shopId:int}")]
        public IActionResult Post(int shopId, MerchOverviewDto merch)
        {
            bool isCreated = _adminApiService.CreateMerch(shopId, merch);
            return isCreated ? Created() :
                StatusCode(StatusCodes.Status500InternalServerError);
        }

        // PUT api/<MerchController>/3
        [HttpPut("{merchId:int}")]
        public IActionResult Put(int merchId, string name)
        {
            bool isChanged = _adminApiService.ChangeMerchName(merchId, name);
            return isChanged ? Ok() : NotFound();
        }

        // DELETE api/<MerchController>/1/3
        [HttpDelete("{merchId:int}")]
        public IActionResult Delete(int merchId)
        {
            if (_adminApiService.DeleteMerch(merchId))
                return Ok();
            else
                return NotFound();
        }

        [HttpPost("{merchId:int}/price")]
        public IActionResult PostPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {
            bool isAdded = _adminApiService.AddPreviousPrice(merchId, timestampedPrice);

            if (isAdded)
                return Ok();
            else
                return NotFound();
        }


        [HttpPost("{merchId:int}/price/current")]
        public IActionResult SetCurrentPrice(int merchId, decimal currentPrice)
        {
            bool isPriceSet = _adminApiService.SetCurrentPrice(merchId, currentPrice);
            if (isPriceSet)
                return Ok();
            else
                return Conflict();
        }

        [HttpDelete("price/{timestampedPriceId:int}")]
        public IActionResult RemoveTimestampedPrice(int timestampedPriceId)
        {
            bool isRemoved = _adminApiService.RemoveTimestampedPrice(timestampedPriceId);
            if (isRemoved)
                return Ok();
            else
                return NotFound();
        }

        [HttpDelete("{merchId:int}/price")]
        public IActionResult ClearOldPrices(int merchId)
        {
            bool isRemoved = _adminApiService.ClearOldPrices(merchId);
            if (isRemoved)
                return Ok();
            else
                return Conflict();
        }


        [HttpGet("citilink/{citilinkMerchCode}")]
        public DetailedMerchDto? GetCitilinkMerch(string citilinkMerchCode)
        {
            return _adminApiService.GetDetailedCitilinkMerch(citilinkMerchCode);
        }

    }
}
