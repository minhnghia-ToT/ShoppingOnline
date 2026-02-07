using ShoppingOnline.DTOs.Products;

namespace ShoppingOnline.DTOs.Admin.Products
{
    public class AdminProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }
        public string Status { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<ProductImageDto> Images { get; set; }
    }
}