namespace OnlineShopping.Models.DTOs
{
    public class BannerDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}