namespace ShoppingOnline.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }
        public string Status { get; set; } = "InStock";

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ProductImage> Images { get; set; }
    }
}
