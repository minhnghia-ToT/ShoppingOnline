using OnlineShopping.Models.DTOs;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IBannerService
    {

        Task CreateBannerAsync(CreateBannerDTO dto);
        Task ToggleBannerAsync(int id);
        Task DeleteBannerAsync(int id);
        Task<List<BannerDTO>> GetAllBannerAsync();
        Task<BannerDTO?> GetBannerByIdAsync(int id);
    }
}