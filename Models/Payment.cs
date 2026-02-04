namespace ShoppingOnline.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
