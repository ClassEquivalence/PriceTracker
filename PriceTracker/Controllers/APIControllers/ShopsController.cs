using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels.ForAPI.Shop;
using PriceTracker.Models.Services.Mapping.MicroMappers;
using PriceTracker.Models.Services.ShopService;
using PriceTracker.Routing;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;
        private readonly ILogger _logger;


        // TODO: [ARCH] эти поля должны находиться в DTO Composer'е, и построение DTO
        // моделей не должно быть здесь.
        // Также обратный маппинг не должен выполняться в контроллере.
        private readonly IShopToDtoMapper _shopToDtoMapper;
        private readonly APILinkBuilder _linkBuilder;

        public ShopsController(ILogger<Program> logger, IShopService shopService,
            IShopToDtoMapper shopDtoMapper, APILinkBuilder linkBuilder)
        {
            _logger = logger;
            _shopService = shopService;
            _shopToDtoMapper = shopDtoMapper;
            _linkBuilder = linkBuilder;
        }

        // GET api/<ShopsController>
        [HttpGet]
        public IActionResult GetShops()
        {
            return Ok(_shopService.Shops.Select(_shopToDtoMapper.ToShopName));
        }

        // GET api/<ShopsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var shop = _shopService.GetShopById(id);
            var link = _linkBuilder.GetShopMerchesPath(id);
            return shop != null ?
                Ok(_shopToDtoMapper.ToShopOverview(shop, link)) 
                : NotFound();
        }

        // POST api/<ShopsController>
        [HttpPost]
        public IActionResult PostShop(ShopNameDto shop)
        {
            if(string.IsNullOrWhiteSpace(shop.Name))
            {
                return BadRequest("Название магазина не может быть пустым.");
            }
            bool isAdded = _shopService.AddShop(new(shop.Name, []));

            if (isAdded)
                return CreatedAtAction(nameof(Get), new {id = shop.Id }, shop);
            else
                return Conflict();
        }

        // PUT api/<ShopsController>/5
        [HttpPut("{id}")]
        public IActionResult ChangeName(int id, string shopName)
        {
            var shop = _shopService.GetShopById(id);
            bool isNameChanged = shop!=null && _shopService.ChangeShopName(shop, shopName);
            if (isNameChanged)
                return Ok();
            else
                return Conflict();
        }

        // DELETE api/<ShopsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool isRemoved = _shopService.RemoveShopById(id);
            if (isRemoved)
                return NoContent();
            else
                return NotFound();
        }
    }
}
