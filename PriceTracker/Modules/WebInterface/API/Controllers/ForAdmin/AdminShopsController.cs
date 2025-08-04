using Microsoft.AspNetCore.Mvc;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.API.DTOModels.Shop;
using PriceTracker.Modules.WebInterface.API.Filters;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Routing;
using PriceTracker.Modules.WebInterface.API.Services.ShopService;





// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Modules.WebInterface.API.Controllers.ForAdmin
{
    [Route(ControllerRoutes.AdminShopControllerRoute)]
    [ApiController]
    [AdminAPIAuthorization]
    public class AdminShopsController : ControllerBase
    {
        private readonly ShopService _service;

        private readonly ILogger _logger;


        public AdminShopsController(ILogger<Program> logger, IShopRepositoryFacade repository,
            IWebInterfaceMapperProvider mapperProvider)
        {
            _service = new(logger, repository, mapperProvider);

            _logger = logger;
        }

        // As ShopNameDto
        [HttpGet]
        public IActionResult GetShops()
        {
            return Ok(_service.GetShops());
        }

        // As ShopOverviewDto
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var shop = _service.GetShop(id);
            return shop != null ?
                Ok(shop) : NotFound();
        }


        [HttpPost]
        public IActionResult PostShop(ShopNameDto shop)
        {
            _service.CreateShop(shop);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult ChangeName(int id, string shopName)
        {
            bool nameChanged = _service.ChangeShopName(id, shopName);
            return nameChanged ? Ok() : Conflict();
        }

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
