namespace ShoppingOnline.DTOs.ProductImages
{
    public class UploadProductImagesDto
    {
        public List<IFormFile> Images { get; set; } = new();
    }
}