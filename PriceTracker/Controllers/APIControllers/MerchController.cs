using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels;
using PriceTracker.Models.DTOModels.ForAPI.Merch;
using PriceTracker.Models.Services.Mapping.MicroMappers;
using PriceTracker.Models.Services.MerchService;
using PriceTracker.Models.Services.ShopService;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// TODO: СДЕЛАТЬ НОРМАЛЬНУЮ МАРШРУТИЗАЦИЮ!
// TODO: В Ok(), наверное, следует возвращать созданный объект. Проверить на соответствие REST.

namespace PriceTracker.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchController : ControllerBase
    {
        private readonly IMerchService _merchService;
        private readonly ILogger _logger;


        // TODO: [ARCH] это поле должно находиться в DTO Composer'е, и построение DTO
        // моделей не должно быть здесь.
        // Также обратный маппинг не должен выполняться в контроллере.
        private readonly IMerchToDtoMapper _merchMapper;

        private readonly IShopService _shopService;
        public MerchController(ILogger<Program> logger, IMerchService merchService, IShopService shopService,
            IMerchToDtoMapper merchMapper)
        {
            _logger = logger;
            _merchService = merchService;
            _merchMapper = merchMapper;
            _shopService = shopService;
        }

        // GET: api/<MerchController>
        [HttpGet("shop/{shopId:int}")]
        public IEnumerable<MerchOverviewDto> GetMerchesOfShop(int shopId)
        {
            var merches = _merchService.GetMerchesOfShop(shopId).Select(_merchMapper.ToMerchOverview);
            return merches;
        }

        // GET api/<MerchController>/1/3
        [HttpGet("{merchId:int}")]
        public DetailedMerchDto? Get(int merchId)
        {
            var merchModel = _merchService.GetMerch(merchId);
            return merchModel != null ? _merchMapper.ToDetailedMerch(merchModel) : null;
        }

        // POST api/<MerchController>/1
        [HttpPost("{shopId:int}")]
        public IActionResult Post(int shopId, MerchOverviewDto merch)
        {
            var shop = _shopService.GetShopById(shopId);
            if (shop == null)
                return NotFound();
            bool isAdded = _merchService.TryCreate(new MerchModel(merch.Name, new TimestampedPrice(merch.CurrentPrice, DateTime.Now),
                shop));
            return isAdded ? Ok() : Conflict();
        }

        // PUT api/<MerchController>/3
        [HttpPut("{merchId:int}")]
        public IActionResult Put(int merchId, string name)
        {
            bool isChanged = _merchService.TryChangeName(merchId, name);
            return isChanged? Ok() : NotFound();
        }

        // DELETE api/<MerchController>/1/3
        [HttpDelete("{merchId:int}")]
        public IActionResult Delete(int merchId)
        {
            if (_merchService.TryDelete(merchId))
                return Ok();
            else
                return NotFound();
        }

        [HttpPost("{merchId:int}/price")]
        public IActionResult PostPrice(int merchId, TimestampedPriceDTO timestampedPrice)
        {
            DateTime dateTime = timestampedPrice.DateTime == default?
                DateTime.Now : timestampedPrice.DateTime;

            bool isAdded = _merchService.TryAddTimestampedPrice(merchId, 
                new TimestampedPrice(timestampedPrice.Price, dateTime));
            if (isAdded)
                return Ok();
            else
                return NotFound();
        }


        [HttpPost("{merchId:int}/price/current")]
        public IActionResult SetCurrentPrice(int merchId, decimal currentPrice)
        {
            bool isPriceSet = _merchService.SetCurrentPrice(merchId, currentPrice) ;
            if (isPriceSet)
                return Ok();
            else
                return Conflict();
        }

        [HttpDelete("price/{timestampedPriceId:int}")]
        public IActionResult RemoveTimestampedPrice(int timestampedPriceId)
        {
            bool isRemoved = _merchService.RemoveSingleTimestampedPrice(timestampedPriceId);
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
    }
}
