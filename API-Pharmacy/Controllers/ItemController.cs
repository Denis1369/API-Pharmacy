using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        // GET: api/<ItemController>
        [HttpGet("get_items")]
        public List<Item> GetListItem()
        {
            return Program._context.Items.Include(i => i.ItemBrand).ToList();
        }
    }
}
