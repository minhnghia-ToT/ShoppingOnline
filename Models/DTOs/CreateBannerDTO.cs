using Microsoft.AspNetCore.Http;

namespace OnlineShopping.Models.DTOs
{
    public class CreateBannerDTO
    {
        public IFormFile Image { get; set; } = null!;
    }
}