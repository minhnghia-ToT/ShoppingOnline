using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.Admin.Products;
using ShoppingOnline.DTOs.Products;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class AdminProductService : IAdminProductService
    {
        private readonly ApplicationDbContext _context;

        public AdminProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<AdminProductDto> Items, int Total)> GetListAsync(ProductQueryDto query)
        {
            var q = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
                q = q.Where(p => p.Name.Contains(query.Keyword));

            if (query.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == query.CategoryId);

            if (!string.IsNullOrEmpty(query.Status))
                q = q.Where(p => p.Status == query.Status);

            var total = await q.CountAsync();

            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new AdminProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Images = p.Images.Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        IsMain = i.IsMain
                    }).ToList()
                })
                .ToListAsync();

            return (items, total);
        }

        public async Task<AdminProductDto> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) throw new Exception("Product not found");

            return new AdminProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                StockQuantity = p.StockQuantity,
                Status = p.Status,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Images = p.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList()
            };
        }

        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new Exception("Product not found");

            // ===== Update thông tin cơ bản =====
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPrice = dto.DiscountPrice;
            product.StockQuantity = dto.StockQuantity;
            product.Status = dto.Status;
            product.CategoryId = dto.CategoryId;

            // ===== Thêm hình ảnh mới nếu có =====
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var img in dto.NewImages)
                {
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = img.ImageUrl,
                        IsMain = img.IsMain,
                        ProductId = product.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) throw new Exception("Product not found");

            p.Status = p.Status == "Disabled" ? "InStock" : "Disabled";
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) throw new Exception("Product not found");

            p.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var img = await _context.ProductImages.FindAsync(imageId);
            if (img == null) throw new Exception("Image not found");

            _context.ProductImages.Remove(img);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new Exception("Product not found");

            // Nếu muốn xóa luôn images liên quan
            if (product.Images != null && product.Images.Any())
            {
                _context.ProductImages.RemoveRange(product.Images);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}