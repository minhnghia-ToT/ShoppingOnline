using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.ProductImages;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductImageService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===============================
        // Upload images
        // ===============================
        public async Task UploadImagesAsync(int productId, UploadProductImagesDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new Exception("Product not found");

            if (dto.Images == null || dto.Images.Count == 0)
                throw new Exception("No images uploaded");

            var uploadPath = Path.Combine(
                _env.WebRootPath,
                "uploads",
                "products",
                productId.ToString()
            );

            Directory.CreateDirectory(uploadPath);

            bool hasMainImage = product.Images.Any(i => i.IsMain);

            foreach (var file in dto.Images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                product.Images.Add(new ProductImage
                {
                    ImageUrl = $"/uploads/products/{productId}/{fileName}",
                    IsMain = !hasMainImage,
                    ProductId = productId
                });

                hasMainImage = true;
            }

            await _context.SaveChangesAsync();
        }

        // ===============================
        // Replace image
        // ===============================
        public async Task ReplaceImageAsync(int productImageId, ReplaceProductImageDto dto)
        {
            var image = await _context.ProductImages.FindAsync(productImageId);
            if (image == null)
                throw new Exception("Image not found");

            var oldFilePath = Path.Combine(
                _env.WebRootPath,
                image.ImageUrl.TrimStart('/')
            );

            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);

            var folder = Path.GetDirectoryName(oldFilePath)!;
            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var newFilePath = Path.Combine(folder, newFileName);

            using var stream = new FileStream(newFilePath, FileMode.Create);
            await dto.Image.CopyToAsync(stream);

            image.ImageUrl =
                Path.Combine(Path.GetDirectoryName(image.ImageUrl)!, newFileName)
                .Replace("\\", "/");

            await _context.SaveChangesAsync();
        }

        // ===============================
        // Set main image
        // ===============================
        public async Task SetMainImageAsync(int productImageId)
        {
            var image = await _context.ProductImages.FindAsync(productImageId);
            if (image == null)
                throw new Exception("Image not found");

            var images = await _context.ProductImages
                .Where(i => i.ProductId == image.ProductId)
                .ToListAsync();

            foreach (var img in images)
                img.IsMain = false;

            image.IsMain = true;

            await _context.SaveChangesAsync();
        }

        // ===============================
        // Delete image
        // ===============================
        public async Task DeleteImageAsync(int productImageId)
        {
            var image = await _context.ProductImages.FindAsync(productImageId);
            if (image == null)
                throw new Exception("Image not found");

            var images = await _context.ProductImages
                .Where(i => i.ProductId == image.ProductId)
                .ToListAsync();

            if (images.Count <= 1)
                throw new Exception("Product must have at least one image");

            _context.ProductImages.Remove(image);

            var filePath = Path.Combine(
                _env.WebRootPath,
                image.ImageUrl.TrimStart('/')
            );

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (image.IsMain)
            {
                images.First(i => i.Id != productImageId).IsMain = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}