namespace API_Pharmacy.DTO
{
    public class UpdateItemRequest
    {
        public int ItemId { get; set; }
        public string? ItemTitle { get; set; }
        public int? ItemBrandId { get; set; }
        public string? ItemDesc { get; set; }
        public string? ItemImg { get; set; }
        public int? ItemCount { get; set; }
        public int? ItemPrice { get; set; }
        public string? ItemStatus { get; set; }
    }
}
