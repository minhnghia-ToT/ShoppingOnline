namespace ShoppingOnline.Models.DTOs.Order_dto_ad
{
    public class OrderAdminDTO
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class OrderDetailAdminDTO
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public string ShippingAddress { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItemAdminDTO> Items { get; set; }
    }

    public class OrderItemAdminDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; }
    }
}
