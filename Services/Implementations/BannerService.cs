using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.DTOs;
using OnlineShopping.Services.Interfaces;
using ShoppingOnline.Data;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BannerService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<List<BannerDTO>> GetAllBannerAsync()
        {
            return await _context.Banners
                .Select(b => new BannerDTO
                {
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    IsActive = b.IsActive
                })
                .ToListAsync();
        }

        public async Task<BannerDTO?> GetBannerByIdAsync(int id)
        {
            return await _context.Banners
                .Where(b => b.Id == id)
                .Select(b => new BannerDTO
                {
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    IsActive = b.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateBannerAsync(CreateBannerDTO dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
                throw new Exception("Image is required");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads/banners");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var banner = new Banner
            {
                ImageUrl = "/uploads/banners/" + fileName,
                IsActive = true
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleBannerAsync(int id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);

            if (banner == null)
                throw new Exception("Banner not found");

            banner.IsActive = !banner.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBannerAsync(int id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);

            if (banner == null)
                throw new Exception("Banner not found");

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}