namespace API_Pharmacy.DTO
{
    public class UpdateClientStatusRequest
    {
        public int ClientId { get; set; }
        public string ClientStatus { get; set; } = "активен";
    }
}
