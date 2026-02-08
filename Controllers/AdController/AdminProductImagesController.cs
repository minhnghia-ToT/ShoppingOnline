using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DTOs.ProductImages;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImagesController(IProductImageService service)
        {
            _service = service;
        }

        // POST /api/admin/products/{id}/images
        [HttpPost("products/{id}/images")]
        public async Task<IActionResult> UploadImages(
            int id,
            [FromForm] UploadProductImagesDto dto)
        {
            await _service.UploadImagesAsync(id, dto);
            return Ok("Images uploaded successfully");
        }

        // PUT /api/admin/product-images/{id}
        [HttpPut("product-images/{id}")]
        public async Task<IActionResult> ReplaceImage(
            int id,
            [FromForm] ReplaceProductImageDto dto)
        {
            await _service.ReplaceImageAsync(id, dto);
            return Ok("Image replaced successfully");
        }

        // PUT /api/admin/product-images/{id}/set-main
        [HttpPut("product-images/{id}/set-main")]
        public async Task<IActionResult> SetMainImage(int id)
        {
            await _service.SetMainImageAsync(id);
            return Ok("Main image updated");
        }

        // DELETE /api/admin/product-images/{id}
        [HttpDelete("product-images/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await _service.DeleteImageAsync(id);
            return Ok("Image deleted successfully");
        }
    }
}