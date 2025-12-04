using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        [HttpGet("get_brands")]
        public List<Brand> GetListBrand()
        {
            return Program._context.Brands.ToList();
        }

        [HttpPut("edit")]
        public IActionResult EditPost([FromBody] Brand editBrand)
        {
            var existingBrand = Program._context.Brands.FirstOrDefault(b => b.BrandId == editBrand.BrandId);
            if (existingBrand == null)
            {
                return BadRequest("Бренд не найден");
            }

            if (existingBrand.BrandName == editBrand.BrandName)
            {
                return Ok("Название не изменено");
            }

            if (Program._context.Brands.FirstOrDefault(b => b.BrandName == editBrand.BrandName) != null)
            {
                return BadRequest("Название бренда уже используется");
            }

            existingBrand.BrandName = editBrand.BrandName;
            Program._context.SaveChanges();

            return Ok("Название бренда успешно обновлено");
        }
    }
}
