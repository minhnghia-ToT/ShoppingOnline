namespace ShoppingOnline.DTOs.Admin.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }
        public string Status { get; set; }

        public int CategoryId { get; set; }

        public List<UpdateProductImageDto>? NewImages { get; set; }
    }
    public class UpdateProductImageDto
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }
}