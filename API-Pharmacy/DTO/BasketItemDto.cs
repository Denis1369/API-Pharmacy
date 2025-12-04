namespace API_Pharmacy.DTO
{
    public class BasketItemDto
    {
        public int BasketItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string ItemImg { get; set; } = string.Empty;
        public int ItemPrice { get; set; }
        public int BasketItemCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
