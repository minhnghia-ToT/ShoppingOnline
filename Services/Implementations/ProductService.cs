using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.DTOs.UserDOTs;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.Products;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetProductsAsync(ProductQueryDto query)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Status == "InStock" || p.Status == "Active");

            if (!string.IsNullOrEmpty(query.Search))
            {
                products = products.Where(p =>
                    p.Name.Contains(query.Search) ||
                    p.Description.Contains(query.Search));
            }

            if (query.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == query.CategoryId);
            }

            if (!string.IsNullOrEmpty(query.SortBy) && query.SortBy == "price")
            {
                products = products.OrderBy(p => p.Price);
            }

            var result = await products
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    MainImage = p.Images
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return result;
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            if (product.Status != "InStock" && product.Status != "Active")
                return null;

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                StockQuantity = product.StockQuantity,
                Status = product.Status,
                CategoryName = product.Category.Name,
                Images = product.Images.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p =>
                    (p.Status == "InStock" || p.Status == "Active") &&
                    (p.Name.Contains(keyword) ||
                     p.Description.Contains(keyword)))
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    MainImage = p.Images
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p =>
                    (p.Status == "InStock" || p.Status == "Active") &&
                    p.CategoryId == categoryId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    MainImage = p.Images
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<bool> CheckStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return false;

            if (product.Status != "InStock" && product.Status != "Active")
                return false;

            return product.StockQuantity >= quantity;
        }
    }
}