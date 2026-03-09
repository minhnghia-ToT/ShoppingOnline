namespace ShoppingOnline.DTOs.Orders
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public List<OrderItemResponseDTO> Items { get; set; }
    }
}