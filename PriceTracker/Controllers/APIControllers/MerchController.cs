using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.BaseAppModels;
using PriceTracker.Models.BaseAppModels.ShopCollections;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchController : ControllerBase
    {
        public IShopCollection ShopCollection { get; set; }
        public ILogger Logger { get; set; }
        public MerchController(ILogger<Program> logger, IShopCollection shopCollection)
        {
            Logger = logger;
            ShopCollection = shopCollection;

        }

        // GET: api/<MerchController>
        [HttpGet("{shopId:int}")]
        public IEnumerable<IShopMerch> Get(int shopId)
        {
            var shop = ShopCollection.GetShopById(shopId);
            if (shop == null)
                return Enumerable.Empty<IShopMerch>();
            else
                return shop.Merches;
        }

        // GET api/<MerchController>/1/3
        [HttpGet("{shopId:int}/{merchId:int}")]
        public IShopMerch? Get(int shopId, int merchId)
        {
            return TryGetMerch(shopId, merchId);
        }

        // POST api/<MerchController>/1
        [HttpPost("{shopId:int}")]
        public IActionResult Post(int shopId, string name, decimal currentPrice)
        {
            var shop = ShopCollection.GetShopById(shopId);
            bool isAdded = false;
            if(shop!=null)
                isAdded = shop.AddMerch(new ShopMerch(name, currentPrice));
            return isAdded ? Ok() : Conflict();
        }

        // PUT api/<MerchController>/1/3
        [HttpPut("{shopId:int}/{merchId:int}")]
        public IActionResult Put(int shopId, int merchId, string name)
        {
            var shop = ShopCollection.GetShopById(shopId);
            if (shop != null)
            {
                if (shop.ChangeMerchName(merchId, name))
                    return Ok();
                else
                    return Conflict();
            }
            return NotFound();
        }

        // DELETE api/<MerchController>/1/3
        [HttpDelete("{shopId:int}/{merchId:int}")]
        public IActionResult Delete(int shopId, int merchId)
        {
            var shop = ShopCollection.GetShopById(shopId);
            if (shop != null && shop.RemoveMerch(merchId))
                return Ok();
            else return NotFound();
        }

        [HttpPost("{shopId:int}/{merchId:int}")]
        public IActionResult PostPrice(int shopId, int merchId, decimal price, DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;

            var merch = TryGetMerch(shopId, merchId);
            if (merch != null)
            {
                merch.PriceTrack.AddPrice(new TimedPrice { Price = price, DateTime = dateTime });
                return Ok();
            }
            else
                return NotFound();
        }

        protected IShopMerch? TryGetMerch(int shopId, int merchId)
        {
            var shop = ShopCollection.GetShopById(shopId);
            if (shop != null)
            {
                var merch = shop.GetMerch(merchId);
                if (merch != null)
                    return merch;
            }
            return null;
        }

    }
}
