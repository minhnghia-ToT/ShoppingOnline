namespace OnlineShopping.Models.DTOs
{
    public class BuyNowDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string PaymentMethod { get; set; } = null!;
    }
}
