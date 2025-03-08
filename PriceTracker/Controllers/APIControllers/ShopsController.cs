using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.BaseModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        public IShopCollection ShopCollection { get; set; }
        public ILogger Logger { get; set; }
        public ShopsController(ILogger<Program> logger, IShopCollection shopCollection) 
        {
            Logger = logger;
            ShopCollection = shopCollection;
            
        }

        // GET api/<ShopsController>
        [HttpGet]
        public IEnumerable<Shop> GetShops()
        {
            return ShopCollection.GetAll();
        }

        // GET api/<ShopsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var shop = ShopCollection.GetShopById(id);
            return shop != null ? Content(shop.ToString()) : NotFound();
        }

        // POST api/<ShopsController>
        [HttpPost]
        public IActionResult PostShop(string shopName)
        {
            var shop = new Shop(shopName, Logger, new List<ShopMerch>());
            bool isAdded = ShopCollection.AddShop(shop);
            if (isAdded)
                return Ok();
            else
                return Conflict();
        }

        // PUT api/<ShopsController>/5
        [HttpPut("{id}")]
        public IActionResult ChangeName(int id, string shopName)
        {
            var shop = ShopCollection.GetShopById(id);
            bool isNameChanged = ShopCollection.ChangeShopName(shop, shopName);
            if (isNameChanged)
                return Ok();
            else
                return Conflict();
        }

        // DELETE api/<ShopsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool isRemoved = ShopCollection.RemoveShopById(id);
            if (isRemoved)
                return Ok();
            else
                return NotFound();
        }
    }
}
