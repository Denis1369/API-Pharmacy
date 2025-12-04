namespace API_Pharmacy.DTO
{
    public class UpdateStatusOnRequest
    {
        public int ItemId { get; set; }
        public string ItemStatusOn { get; set; } = "да";
    }
}
