using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketItemController : ControllerBase
    {
        [HttpGet("{id}")]
        public BasketItem Get(int id)
        {
            return Program._context.BasketItems.FirstOrDefault(i => i.BasketItemId == id);
        }

        [HttpPost("add")]
        public IActionResult AddBasketItem([FromBody] AddItem addItem) 
        {


            return Ok();
        }
    }

    public class AddItem
    {
        public int? BasketId { get; set; }
        public int? ItemId { get; set; }
        public int? BasketItemCount { get; set; }
    }
}
