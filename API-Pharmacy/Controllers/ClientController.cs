using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpGet("get_clients")]
        public async Task<List<Client>> GetListClients()
        {
            var context = new PharmacyDbContext();

            var list = await context.Clients.ToListAsync();
            return list;
        }

        [HttpPost("registration")]
        public IActionResult RegistrationPost([FromBody] Client client)
        {

            if (string.IsNullOrWhiteSpace(client.ClientEmail) || !client.ClientEmail.Contains("@"))
            {
                return BadRequest("Некорректный формат email");
            }

            if (Program._context.Clients.FirstOrDefault(c => c.ClientEmail == client.ClientEmail) != null) 
            {
                return BadRequest("Email использован");
            }

            //(bool flag, string text) =  Client.ValidatePassword(client.ClientPassword);
            //if (!flag) 
            //{
            //    return BadRequest(text);
            //}

            //if (client.ClientLastName.Length < 2) 
            //{
            //    return BadRequest("Длина фамилии должна быть больше 2 букв");
            //}

            //if (client.ClientName.Length < 2)
            //{
            //    return BadRequest("Длина имени должна быть больше 2 букв");
            //}

            Program._context.Clients.Add(client);

            Program._context.SaveChanges();

            Client client1 = Program._context.Clients.FirstOrDefault(c => c.ClientEmail == client.ClientEmail && c.ClientPassword == client.ClientPassword);

            var basket = new Basket(){
                BasketClientId = client1.ClientId,
                BasketStatus = "активная"
            };

            Program._context.Baskets.Add(basket);
            
            Program._context.SaveChangesAsync();

            return Ok(client1);
        }

        [HttpPost("login")]
        public IActionResult LoginPost([FromBody] Login login)
        {
            var context = new PharmacyDbContext();
            Client client = context.Clients.FirstOrDefault(c => c.ClientEmail == login.email && c.ClientPassword == login.pass);
            if (client == null) 
            {
                return BadRequest("Неверный email или пароль");
            }

            if (client.ClientStatus == "заблокирован")
            {
                return Unauthorized("Аккаунт заблокирован");
            }

            return Ok(client);
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateClientProfile([FromBody] Client updatedClient)
        {
            try
            {
                var client = await Program._context.Clients.FindAsync(updatedClient.ClientId);
                if (client == null)
                {
                    return NotFound("Пользователь не найден");
                }

                var existingClientWithSameEmail = await Program._context.Clients
                    .Where(c => c.ClientEmail == updatedClient.ClientEmail && c.ClientId != updatedClient.ClientId)
                    .FirstOrDefaultAsync();

                if (existingClientWithSameEmail != null)
                {
                    return BadRequest("Email уже используется другим пользователем.");
                }

                client.ClientEmail = updatedClient.ClientEmail;
                client.ClientLastName = updatedClient.ClientLastName;
                client.ClientName = updatedClient.ClientName;

                Program._context.SaveChanges();

                return Ok("Профиль успешно обновлён");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("updateStatus")]
        public async Task<IActionResult> UpdateClientStatus([FromBody] UpdateClientStatusRequest request)
        {
            try
            {
                var client = await Program._context.Clients.FindAsync(request.ClientId);
                if (client == null)
                {
                    return NotFound("Клиент не найден");
                }

                client.ClientStatus = request.ClientStatus;
                await Program._context.SaveChangesAsync();

                return Ok("Статус успешно обновлён");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

    }
}
