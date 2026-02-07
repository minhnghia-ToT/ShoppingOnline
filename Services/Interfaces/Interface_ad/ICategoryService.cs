using ShoppingOnline.DTOs.Categories;

namespace ShoppingOnline.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto> UpdateAsync(UpdateCategoryDto dto);

        Task<bool> ToggleStatusAsync(int categoryId);

        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<List<CategoryAdminListDto>> GetAllAsync();
    }
}