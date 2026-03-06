using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DTOs.Products;
using ShoppingOnline.Services.Interfaces;
using OnlineShopping.Models.DTOs.UserDOTs;
namespace ShoppingOnline.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryDto query)
        {
            var products = await _productService.GetProductsAsync(query);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _productService.SearchProductsAsync(keyword);
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("check-stock")]
        public async Task<IActionResult> CheckStock(int productId, int quantity)
        {
            var result = await _productService.CheckStockAsync(productId, quantity);

            if (!result)
                return BadRequest("Product out of stock");

            return Ok("Product available");
        }
    }
}