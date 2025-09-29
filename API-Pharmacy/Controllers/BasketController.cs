using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientBaskets(int clientId)
        {
            try
            {
                var baskets = await Program._context.Baskets
                    .Where(b => b.BasketClientId == clientId)
                    .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Item)
                    .ThenInclude(i => i.ItemBrand)
                    .Select(b => new
                    {
                        b.BasketId,
                        b.BasketDate,
                        b.BasketStatus,
                        ItemsCount = b.BasketItems.Count,
                        TotalAmount = b.BasketItems.Sum(bi => bi.Item.ItemPrice * bi.BasketItemCount)
                    })
                    .ToListAsync();

                return Ok(baskets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении корзин клиента: {ex.Message}");
            }
        }

        // GET: api/Basket/5 - получить детали корзины
        [HttpGet("{basketId}")]
        public async Task<IActionResult> GetBasket(int basketId)
        {
            try
            {
                var basket = await Program._context.Baskets
                    .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Item)
                    .ThenInclude(i => i.ItemBrand)
                    .FirstOrDefaultAsync(b => b.BasketId == basketId);

                if (basket == null)
                {
                    return NotFound($"Корзина с ID {basketId} не найдена");
                }

                var result = new
                {
                    basket.BasketId,
                    basket.BasketDate,
                    basket.BasketStatus,
                    basket.BasketClientId,
                    Items = basket.BasketItems.Select(bi => new
                    {
                        bi.BasketItemId,
                        bi.ItemId,
                        ItemTitle = bi.Item.ItemTitle,
                        ItemPrice = bi.Item.ItemPrice,
                        ItemBrand = bi.Item.ItemBrand != null ? bi.Item.ItemBrand.BrandName : null,
                        bi.BasketItemCount,
                        TotalPrice = bi.Item.ItemPrice * bi.BasketItemCount
                    }),
                    TotalAmount = basket.BasketItems.Sum(bi => bi.Item.ItemPrice * bi.BasketItemCount),
                    TotalItems = basket.BasketItems.Sum(bi => bi.BasketItemCount)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении корзины: {ex.Message}");
            }
        }

        // POST: api/Basket/checkout - оформление покупки
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutBasket([FromBody] CheckoutRequest request)
        {
            using var transaction = await Program._context.Database.BeginTransactionAsync();

            try
            {
                // 1. Находим текущую корзину
                var currentBasket = await Program._context.Baskets
                    .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Item)
                    .FirstOrDefaultAsync(b => b.BasketId == request.BasketId);

                if (currentBasket == null)
                {
                    return NotFound($"Корзина с ID {request.BasketId} не найдена");
                }

                // 2. Проверяем, что корзина не пустая
                if (!currentBasket.BasketItems.Any())
                {
                    return BadRequest("Корзина пуста. Добавьте товары перед оформлением заказа.");
                }

                // 3. Проверяем доступность товаров и обновляем остатки
                foreach (var basketItem in currentBasket.BasketItems)
                {
                    var item = basketItem.Item;
                    if (item.ItemCount < basketItem.BasketItemCount)
                    {
                        return BadRequest($"Недостаточно товара '{item.ItemTitle}'. Доступно: {item.ItemCount}, запрошено: {basketItem.BasketItemCount}");
                    }

                    // Уменьшаем количество товара на складе
                    item.ItemCount -= basketItem.BasketItemCount;
                    Program._context.Items.Update(item);
                }

                // 4. Изменяем статус текущей корзины на "завершена"
                currentBasket.BasketStatus = "completed";
                Program._context.Baskets.Update(currentBasket);

                // 5. Создаем новую корзину для клиента
                var newBasket = new Basket
                {
                    BasketClientId = currentBasket.BasketClientId,
                    BasketDate = DateTime.Now,
                    BasketStatus = "active"
                };

                Program._context.Baskets.Add(newBasket);
                await Program._context.SaveChangesAsync();

                // 6. Фиксируем транзакцию
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
                return StatusCode(500, $"Ошибка при оформлении заказа: {ex.Message}");
            }
        }

        // POST: api/Basket/create - создание новой корзины для клиента
        [HttpPost("create")]
        public async Task<IActionResult> CreateBasket([FromBody] CreateBasketRequest request)
        {
            try
            {
                // Проверяем существование клиента
                var client = await Program._context.Clients.FindAsync(request.ClientId);
                if (client == null)
                {
                    return NotFound($"Клиент с ID {request.ClientId} не найден");
                }

                // Создаем новую корзину
                var newBasket = new Basket
                {
                    BasketClientId = request.ClientId,
                    BasketDate = DateTime.Now,
                    BasketStatus = "active"
                };

                Program._context.Baskets.Add(newBasket);
                await Program._context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Новая корзина создана",
                    BasketId = newBasket.BasketId,
                    ClientId = newBasket.BasketClientId,
                    CreatedDate = newBasket.BasketDate,
                    Status = newBasket.BasketStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании корзины: {ex.Message}");
            }
        }

        // PUT: api/Basket/5/cancel - отмена корзины
        [HttpPut("{basketId}/cancel")]
        public async Task<IActionResult> CancelBasket(int basketId)
        {
            try
            {
                var basket = await Program._context.Baskets
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(b => b.BasketId == basketId);

                if (basket == null)
                {
                    return NotFound($"Корзина с ID {basketId} не найдена");
                }

                if (basket.BasketStatus == "completed")
                {
                    return BadRequest("Нельзя отменить уже завершенную корзину");
                }

                basket.BasketStatus = "cancelled";
                Program._context.Baskets.Update(basket);
                await Program._context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Корзина отменена",
                    BasketId = basket.BasketId,
                    Status = basket.BasketStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при отмене корзины: {ex.Message}");
            }
        }
    }

    public class CheckoutRequest
    {
        public int BasketId { get; set; }
    }

    public class CreateBasketRequest
    {
        public int ClientId { get; set; }
    }
}
