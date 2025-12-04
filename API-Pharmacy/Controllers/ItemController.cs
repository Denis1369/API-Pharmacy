using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        [HttpGet("get_items")]
        public List<Item> GetListItem()
        {
            var list = Program._context.Items.Include(i => i.ItemBrand)
                .Where(i =>i.ItemStatusOn == "да").ToList();
            return list;
        }

        [HttpGet("get_items_admin")]
        public List<Item> GetListItemAdmin()
        {
            var list = Program._context.Items.Include(i => i.ItemBrand).ToList();
            return list;
        }

        [HttpPut("updateStatusOn")]
        public async Task<IActionResult> UpdateStatusOn([FromBody] UpdateStatusOnRequest request)
        {
            try
            {
                var item = await Program._context.Items.FindAsync(request.ItemId);
                if (item == null)
                {
                    return NotFound("Товар не найден");
                }

                item.ItemStatusOn = request.ItemStatusOn;
                await Program._context.SaveChangesAsync();

                return Ok("Статус успешно обновлён");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem([FromBody] Item item)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(item.ItemTitle) || string.IsNullOrWhiteSpace(item.ItemDesc))
                {
                    return BadRequest("Название или описание не может быть пустым");
                }

                if (item.ItemCount == null || item.ItemCount < 1)
                {
                    return BadRequest("Количество должно быть больше 0");
                }

                if (item.ItemPrice == null || item.ItemPrice < 1)
                {
                    return BadRequest("Цена должна быть больше 0");
                }

                if (item.ItemBrandId == null || item.ItemStatus == null) 
                {
                    return BadRequest("Бренд и статус надо выбрать");
                }

                if (string.IsNullOrWhiteSpace(item.ItemImg))
                {
                    return BadRequest("Путь для изображения не может быть пустым");
                }

                var existingItem = await Program._context.Items.FindAsync(item.ItemId);
                if (existingItem == null)
                {
                    return NotFound("Товар не найден");
                }


                existingItem.ItemTitle = item.ItemTitle;
                existingItem.ItemBrandId = item.ItemBrandId;
                existingItem.ItemDesc = item.ItemDesc;
                existingItem.ItemImg = item.ItemImg;
                existingItem.ItemCount = item.ItemCount;
                existingItem.ItemPrice = item.ItemPrice;
                existingItem.ItemStatus = item.ItemStatus;

                await Program._context.SaveChangesAsync();

                return Ok("Товар успешно обновлён");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}
