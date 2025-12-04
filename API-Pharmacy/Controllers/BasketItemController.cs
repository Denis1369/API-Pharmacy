using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
                if (addItem.ItemId == null || addItem.ClientId == null)
                {
                    return BadRequest("ItemId и ClientId обязательны для заполнения");
                }

                var activeBasket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketClientId == addItem.ClientId && b.BasketStatus == "активная");

                if (activeBasket == null)
                {
                    return BadRequest("У клиента нет активной корзины. Создайте новую или восстановите существующую.");
                }

                var basketId = activeBasket.BasketId;

                var item = await Program._context.Items
                    .FirstOrDefaultAsync(i => i.ItemId == addItem.ItemId);

                if (item == null)
                {
                    return NotFound($"Товар с ID {addItem.ItemId} не найден");
                }

                if (item.ItemCount.HasValue && 1 > item.ItemCount.Value)
                {
                    return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}");
                }

                var existingBasketItem = await Program._context.BasketItems
                    .FirstOrDefaultAsync(bi => bi.BasketId == basketId && bi.ItemId == addItem.ItemId);

                if (existingBasketItem != null)
                {
                    var newCount = existingBasketItem.BasketItemCount.GetValueOrDefault() + 1;

                    if (item.ItemCount.HasValue && newCount > item.ItemCount.Value)
                    {
                        return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}, уже в корзине: {existingBasketItem.BasketItemCount}");
                    }

                    existingBasketItem.BasketItemCount = newCount;
                    Program._context.BasketItems.Update(existingBasketItem);
                }
                else
                {
                    var newBasketItem = new BasketItem
                    {
                        BasketId = basketId,
                        ItemId = addItem.ItemId.Value,
                        BasketItemCount = 1
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

        [HttpPost("min")]
        public async Task<IActionResult> MinBasketItem([FromBody] AddItem addItem)
        {
            try
            {
                if (addItem.ItemId == null || addItem.ClientId == null)
                {
                    return BadRequest("ItemId и ClientId обязательны для заполнения");
                }

                var activeBasket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketClientId == addItem.ClientId && b.BasketStatus == "активная");

                if (activeBasket == null)
                {
                    return BadRequest("У клиента нет активной корзины. Создайте новую или восстановите существующую.");
                }

                var basketId = activeBasket.BasketId;

                var item = await Program._context.Items
                    .FirstOrDefaultAsync(i => i.ItemId == addItem.ItemId);

                if (item == null)
                {
                    return NotFound($"Товар с ID {addItem.ItemId} не найден");
                }

                if (item.ItemCount.HasValue && 1 > item.ItemCount.Value)
                {
                    return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}");
                }

                var existingBasketItem = await Program._context.BasketItems
                    .FirstOrDefaultAsync(bi => bi.BasketId == basketId && bi.ItemId == addItem.ItemId);

                if (existingBasketItem != null)
                {
                    var newCount = existingBasketItem.BasketItemCount.GetValueOrDefault() - 1;

                    if (newCount < 1) 
                    {
                        Program._context.BasketItems.Remove(existingBasketItem);
                        await Program._context.SaveChangesAsync();
                        return Ok("Товар успешно удален из корзины");
                    }

                    if (item.ItemCount.HasValue && newCount > item.ItemCount.Value)
                    {
                        return BadRequest($"Недостаточно товара на складе. Доступно: {item.ItemCount}, уже в корзине: {existingBasketItem.BasketItemCount}");
                    }

                    existingBasketItem.BasketItemCount = newCount;
                    Program._context.BasketItems.Update(existingBasketItem);
                }

                await Program._context.SaveChangesAsync();
                return Ok("Успешно изменено колличество в корзину");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении товара в корзину: {ex.Message}");
            }
        }

        [HttpPost("basket")]
        public async Task<IActionResult> PostBasketItems([FromBody] User clientId)
        {
            try
            {
                var activeBasket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketClientId == clientId.ClientId && b.BasketStatus == "активная");

                if (activeBasket == null)
                {
                    return Ok(new List<BasketItemDto>());
                }

                var basketItems = await Program._context.BasketItems
                    .Where(bi => bi.BasketId == activeBasket.BasketId)
                    .Include(bi => bi.Item)
                        .ThenInclude(i => i.ItemBrand)
                    .Select(bi => new BasketItemDto
                    {
                        BasketItemId = bi.BasketItemId,
                        ItemId = bi.ItemId ?? 0,
                        ItemTitle = bi.Item.ItemTitle,
                        ItemImg = bi.Item.ItemImg,
                        ItemPrice = (int)bi.Item.ItemPrice,
                        BasketItemCount = bi.BasketItemCount ?? 0,
                        TotalPrice = (int)bi.Item.ItemPrice * (bi.BasketItemCount ?? 0)
                    })
                    .ToListAsync();

                decimal totalSum = basketItems.Sum(item => item.TotalPrice);

                var result = new BasketWithTotalDto
                {
                    Items = basketItems,
                    TotalSum = totalSum,
                    Basket = activeBasket.BasketId
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении содержимого корзины: {ex.Message}");
            }
        }

        [HttpPut("remove")]
        public async Task<IActionResult> RemoveBasketItem([FromBody] RemoveItem removeItem)
        {
            try
            {
                if (removeItem.ClientId == null || removeItem.ItemId == null)
                {
                    return BadRequest("BasketId и ItemId обязательны для заполнения");
                }

                var activeBasket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketClientId == removeItem.ClientId && b.BasketStatus == "активная");

                var basketItem = await Program._context.BasketItems
                    .FirstOrDefaultAsync(bi => bi.BasketId == activeBasket.BasketId && bi.ItemId == removeItem.ItemId);

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
}
