namespace OnlineShopping.Models.DTOs.UserDOTs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }

        public string Status { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public List<string> Images { get; set; } = new();
    }
}