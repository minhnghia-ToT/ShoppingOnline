namespace ShoppingOnline.DTOs.Orders
{
    public class OrderItemResponseDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = null!;
    }
}