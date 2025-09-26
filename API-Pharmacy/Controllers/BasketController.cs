using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {

        [HttpGet("{id}")]
        public Basket Get(int id)
        {
            return Program._context.Baskets.FirstOrDefault(b=>b.BasketId==id);
        }

        // POST api/<BasketController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BasketController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BasketController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
