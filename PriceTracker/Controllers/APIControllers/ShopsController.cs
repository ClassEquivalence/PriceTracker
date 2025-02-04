using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.BaseModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PriceTracker.Controllers.APIControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<IShop> GetShops()
        {

        }


        // GET: api/<ShopsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ShopsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ShopsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ShopsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ShopsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
