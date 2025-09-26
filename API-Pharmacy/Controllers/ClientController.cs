using Microsoft.AspNetCore.Mvc;
using API_Pharmacy.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
     
        [HttpPost("registration")]
        public IActionResult RegistrationPost([FromBody] Client client)
        {
            if (Program._context.Clients.FirstOrDefault(c => c.ClientEmail == client.ClientEmail) != null) 
            {
                return BadRequest("Email использован");
            }

            (bool flag, string text) =  Client.ValidatePassword(client.ClientPassword);
            if (!flag) 
            {
                return BadRequest(text);
            }

            if (client.ClientLastName.Length < 2) 
            {
                return BadRequest("Длина фамилии должна быть больше 2 букв");
            }

            if (client.ClientName.Length < 2)
            {
                return BadRequest("Длина имени должна быть больше 2 букв");
            }

            Program._context.Clients.Add(client);

            Program._context.SaveChangesAsync();

            return Ok("Успешно");
        }

        [HttpPost("login")]
        public IActionResult LoginPost( string email, string pass)
        {
            if (Program._context.Clients.FirstOrDefault(c => c.ClientEmail == email && c.ClientPassword == pass) == null) 
            {
                return BadRequest("Неверный email или пароль");
            }

            return Ok("Успешно");
        }

        [HttpPut("edit/{id}")]
        public IActionResult EditClient(int id, [FromBody] Client updatedClient)
        {
            var existingClient = Program._context.Clients.FirstOrDefault(c => c.ClientId == id);
            if (existingClient == null)
            {
                return BadRequest("Клиент не найден");
            }

            if (existingClient.ClientEmail != updatedClient.ClientEmail)
            {
                if (Program._context.Clients.FirstOrDefault(c => c.ClientEmail == updatedClient.ClientEmail) != null)
                {
                    return BadRequest("Email использован");
                }
            }

            if (existingClient.ClientPassword != updatedClient.ClientPassword)
            {
                (bool flag, string text) = Client.ValidatePassword(updatedClient.ClientPassword);
                if (!flag)
                {
                    return BadRequest(text);
                }
            }

            if (updatedClient.ClientLastName.Length < 2)
            {
                return BadRequest("Длина фамилии должна быть больше 2 букв");
            }

            if (updatedClient.ClientName.Length < 2)
            {
                return BadRequest("Длина имени должна быть больше 2 букв");
            }

            existingClient.ClientName = updatedClient.ClientName;
            existingClient.ClientLastName = updatedClient.ClientLastName;
            existingClient.ClientEmail = updatedClient.ClientEmail;
            existingClient.ClientPassword = updatedClient.ClientPassword;

            Program._context.SaveChanges();

            return Ok("Данные клиента успешно обновлены");
        }
    }
}
