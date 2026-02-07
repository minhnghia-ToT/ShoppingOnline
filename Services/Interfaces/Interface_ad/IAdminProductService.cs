using ShoppingOnline.DTOs.Admin.Products;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IAdminProductService
    {
        Task<(List<AdminProductDto> Items, int Total)> GetListAsync(ProductQueryDto query);
        Task<AdminProductDto> GetByIdAsync(int id);

        Task<int> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(int id, UpdateProductDto dto);

        Task ToggleStatusAsync(int id);
        Task UpdateStatusAsync(int id, string status);

        Task DeleteImageAsync(int imageId);
    }
}