using ShoppingOnline.DTOs.ProductImages;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IProductImageService
    {
        Task UploadImagesAsync(int productId, UploadProductImagesDto dto);
        Task ReplaceImageAsync(int productImageId, ReplaceProductImageDto dto);
        Task SetMainImageAsync(int productImageId);
        Task DeleteImageAsync(int productImageId);
    }
}