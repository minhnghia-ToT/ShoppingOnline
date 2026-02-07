using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.Categories;
using ShoppingOnline.Models;

namespace ShoppingOnline.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================
        // CREATE CATEGORY
        // ======================
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new Exception("Category name already exists");

            var category = new Category
            {
                Name = dto.Name,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return await MapToResponseDto(category.Id);
        }

        // ======================
        // UPDATE CATEGORY
        // ======================
        public async Task<CategoryResponseDto> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.Id)
                ?? throw new Exception("Category not found");

            var duplicated = await _context.Categories
                .AnyAsync(c => c.Id != dto.Id && c.Name.ToLower() == dto.Name.ToLower());

            if (duplicated)
                throw new Exception("Category name already exists");

            category.Name = dto.Name;
            await _context.SaveChangesAsync();

            return await MapToResponseDto(category.Id);
        }

        // ======================
        // TOGGLE ACTIVE / INACTIVE
        // ======================
        public async Task<bool> ToggleStatusAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId)
                ?? throw new Exception("Category not found");

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();

            return category.IsActive;
        }

        // ======================
        // GET BY ID
        // ======================
        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    ProductCount = c.Products.Count
                })
                .FirstOrDefaultAsync();
        }

        // ======================
        // GET ALL (ADMIN)
        // ======================
        public async Task<List<CategoryAdminListDto>> GetAllAsync()
        {
            return await _context.Categories
                .OrderByDescending(c => c.Id)
                .Select(c => new CategoryAdminListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();
        }

        // ======================
        // PRIVATE MAPPER
        // ======================
        private async Task<CategoryResponseDto> MapToResponseDto(int categoryId)
        {
            return await _context.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    ProductCount = c.Products.Count
                })
                .FirstAsync();
        }
    }
}