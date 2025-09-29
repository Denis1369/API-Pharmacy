using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketItemController : ControllerBase
    {
        [HttpPost("add")]
        public async Task<IActionResult> AddBasketItem([FromBody] AddItem addItem)
        {
            try
            {
                // Проверяем входные данные
                if (addItem.BasketId == null || addItem.ItemId == null || addItem.BasketItemCount == null)
                {
                    return BadRequest("BasketId, ItemId и BasketItemCount обязательны для заполнения");
                }

                if (addItem.BasketItemCount <= 0)
                {
                    return BadRequest("Количество должно быть положительным числом");
                }

                // Проверяем существование товара
                var item = await Program._context.Items
                    .FirstOrDefaultAsync(i => i.ItemId == addItem.ItemId);

                if (item == null)
                {
                    return NotFound($"Товар с ID {addItem.ItemId} не найден");
                }

                // Проверяем существование корзины
                var basket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketId == addItem.BasketId);

                if (basket == null)
                {
                    return NotFound($"Корзина с ID {addItem.BasketId} не найдена");
                }

                // Проверяем, есть ли уже такой товар в корзине
                var existingBasketItem = await Program._context.BasketItems
                    .FirstOrDefaultAsync(bi => bi.BasketId == addItem.BasketId && bi.ItemId == addItem.ItemId);

                if (existingBasketItem != null)
                {
                    // Если товар уже есть в корзине - увеличиваем количество
                    var newCount = existingBasketItem.BasketItemCount.GetValueOrDefault() + addItem.BasketItemCount.Value;

                    // Проверяем, не превышает ли количество доступный запас
                    if (item.ItemCount.HasValue && newCount > item.ItemCount.Value)
                    {
                        return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}, уже в корзине: {existingBasketItem.BasketItemCount}");
                    }

                    existingBasketItem.BasketItemCount = newCount;
                    Program._context.BasketItems.Update(existingBasketItem);
                }
                else
                {
                    // Если товара нет в корзине - добавляем новый
                    // Проверяем, не превышает ли количество доступный запас
                    if (item.ItemCount.HasValue && addItem.BasketItemCount > item.ItemCount.Value)
                    {
                        return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}");
                    }

                    var newBasketItem = new BasketItem
                    {
                        BasketId = addItem.BasketId.Value,
                        ItemId = addItem.ItemId.Value,
                        BasketItemCount = addItem.BasketItemCount.Value
                    };

                    Program._context.BasketItems.Add(newBasketItem);
                }

                await Program._context.SaveChangesAsync();
                return Ok("Товар успешно добавлен в корзину");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении товара в корзину: {ex.Message}");
            }
        }

        // Дополнительный метод для получения содержимого корзины
        [HttpGet("basket/{basketId}")]
        public async Task<IActionResult> GetBasketItems(int basketId)
        {
            try
            {
                var basketItems = await Program._context.BasketItems
                    .Where(bi => bi.BasketId == basketId)
                    .Include(bi => bi.Item)
                    .ThenInclude(i => i.ItemBrand) // Если нужно получить бренд товара
                    .Select(bi => new
                    {
                        BasketItemId = bi.BasketItemId,
                        ItemId = bi.ItemId,
                        ItemTitle = bi.Item.ItemTitle,
                        ItemPrice = bi.Item.ItemPrice,
                        ItemBrand = bi.Item.ItemBrand != null ? bi.Item.ItemBrand.BrandName : null,
                        BasketItemCount = bi.BasketItemCount,
                        TotalPrice = bi.Item.ItemPrice * bi.BasketItemCount
                    })
                    .ToListAsync();

                return Ok(basketItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении содержимого корзины: {ex.Message}");
            }
        }

        // Метод для удаления товара из корзины
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveBasketItem([FromBody] RemoveItem removeItem)
        {
            try
            {
                if (removeItem.BasketId == null || removeItem.ItemId == null)
                {
                    return BadRequest("BasketId и ItemId обязательны для заполнения");
                }

                var basketItem = await Program._context.BasketItems
                    .FirstOrDefaultAsync(bi => bi.BasketId == removeItem.BasketId && bi.ItemId == removeItem.ItemId);

                if (basketItem == null)
                {
                    return NotFound("Товар не найден в корзине");
                }

                Program._context.BasketItems.Remove(basketItem);
                await Program._context.SaveChangesAsync();

                return Ok("Товар успешно удален из корзины");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении товара из корзины: {ex.Message}");
            }
        }
    }

    public class AddItem
    {
        public int? BasketId { get; set; }
        public int? ItemId { get; set; }
        public int? BasketItemCount { get; set; }
    }

    public class RemoveItem
    {
        public int? BasketId { get; set; }
        public int? ItemId { get; set; }
    }
}
