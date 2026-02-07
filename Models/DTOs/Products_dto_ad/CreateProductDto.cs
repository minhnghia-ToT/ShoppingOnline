namespace ShoppingOnline.DTOs.Admin.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }
        public string Status { get; set; } = "Disabled";

        public int CategoryId { get; set; }
    }
}