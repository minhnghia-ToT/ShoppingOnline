namespace ShoppingOnline.Models
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
