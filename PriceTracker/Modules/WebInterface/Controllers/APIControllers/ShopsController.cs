using Microsoft.AspNetCore.Mvc;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop;
using PriceTracker.Modules.WebInterface.Routing;
using PriceTracker.Modules.WebInterface.Services.InterfaceServices;
using PriceTracker.Modules.WebInterface.Services.ShopService;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Modules.WebInterface.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly AdminAPIService _service;

        private readonly ILogger _logger;


        public ShopsController(ILogger<Program> logger, AdminAPIService service)
        {
            _service = service;

            _logger = logger;
        }

        // GET api/<ShopsController>
        [HttpGet]
        public IActionResult GetShops()
        {
            return Ok(_service.GetShops());
        }

        // GET api/<ShopsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var shop = _service.GetShop(id);
            return shop != null ?
                Ok(shop) : NotFound();
        }

        // POST api/<ShopsController>
        [HttpPost]
        public IActionResult PostShop(ShopNameDto shop)
        {
            bool shopCreated = _service.CreateShop(shop);

            if (shopCreated)
                return Created();
            else
                return Conflict();
        }

        // PUT api/<ShopsController>/5
        [HttpPut("{id}")]
        public IActionResult ChangeName(int id, string shopName)
        {
            bool nameChanged = _service.ChangeShopName(id, shopName);
            return nameChanged ? Ok() : Conflict();
        }

        // DELETE api/<ShopsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool isRemoved = _service.DeleteShop(id);
            if (isRemoved)
                return NoContent();
            else
                return NotFound();
        }
    }
}
