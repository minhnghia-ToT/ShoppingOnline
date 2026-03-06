using OnlineShopping.Models.DTOs.UserDOTs;
using ShoppingOnline.DTOs.Products;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync(ProductQueryDto query);

        Task<ProductDetailDto?> GetProductDetailAsync(int id);

        Task<List<ProductDto>> SearchProductsAsync(string keyword);

        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);

        Task<bool> CheckStockAsync(int productId, int quantity);
    }
}