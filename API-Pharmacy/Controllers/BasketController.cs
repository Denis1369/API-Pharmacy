using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutBasket([FromBody] CheckoutRequest request)
        {

            try
            {
                var currentBasket = await Program._context.Baskets
                    .FirstOrDefaultAsync(b => b.BasketClientId == request.ClientId && b.BasketStatus == "активная");


                if (currentBasket == null)
                {
                    return NotFound($"Корзина не найдена");
                }

                if (!currentBasket.BasketItems.Any())
                {
                    return BadRequest("Корзина пуста. Добавьте товары перед оформлением заказа.");
                }

                foreach (var basketItem in currentBasket.BasketItems)
                {
                    var item = basketItem.Item;
                    if (item.ItemCount < basketItem.BasketItemCount)
                    {
                        return BadRequest($"Недостаточно товара '{item.ItemTitle}'. Доступно: {item.ItemCount}, запрошено: {basketItem.BasketItemCount}");
                    }

                    item.ItemCount -= basketItem.BasketItemCount;
                    Program._context.Items.Update(item);
                }

                currentBasket.BasketStatus = "оформлена";
                currentBasket.BasketDate = DateTime.Now;

                Program._context.Baskets.Update(currentBasket);

                var newBasket = new Basket
                {
                    BasketClientId = currentBasket.BasketClientId,
                    BasketStatus = "активная"
                };

                Program._context.Baskets.Add(newBasket);
                await Program._context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Покупка успешно оформлена",
                    CompletedBasketId = currentBasket.BasketId,
                    NewBasketId = newBasket.BasketId,
                    TotalItems = currentBasket.BasketItems.Sum(bi => bi.BasketItemCount),
                    TotalAmount = currentBasket.BasketItems.Sum(bi => bi.Item.ItemPrice * bi.BasketItemCount)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при оформлении заказа: {ex.Message}");
            }
        }
    }
}
