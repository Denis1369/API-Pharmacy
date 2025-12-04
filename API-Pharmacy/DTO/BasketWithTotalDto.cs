namespace API_Pharmacy.DTO
{
    public class BasketWithTotalDto
    {
        public List<BasketItemDto> Items { get; set; } = new();
        public decimal TotalSum { get; set; }
        public int Basket { get; set; }
    }
}
