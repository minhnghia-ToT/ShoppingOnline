using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models.DTOs;
using OnlineShopping.Services.Interfaces;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Controllers.Admin
{
    [Route("api/admin/banners")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBanner()
        {
            var banners = await _bannerService.GetAllBannerAsync();
            return Ok(banners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerById(int id)
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);

            if (banner == null)
                return NotFound("Banner not found");

            return Ok(banner);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDTO dto)
        {
            await _bannerService.CreateBannerAsync(dto);
            return Ok("Banner created successfully");
        }

        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleBanner(int id)
        {
            await _bannerService.ToggleBannerAsync(id);
            return Ok("Banner status updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            await _bannerService.DeleteBannerAsync(id);
            return Ok("Banner deleted successfully");
        }
    }
}