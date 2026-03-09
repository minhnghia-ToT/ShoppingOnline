using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DTOs.Cart;
using ShoppingOnline.Services.Interfaces;
using System.Security.Claims;

namespace ShoppingOnline.Controllers
{
    [Authorize]
    [ApiController]
    [Route("cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDTO dto)
        {
            await _service.AddToCart(GetUserId(), dto);

            return Ok(new { message = "Product added to cart" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart(UpdateCartDTO dto)
        {
            await _service.UpdateCart(GetUserId(), dto);

            return Ok(new { message = "Cart updated successfully" });
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            await _service.RemoveItem(GetUserId(), productId);

            return Ok(new { message = "Item removed from cart" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _service.GetCart(GetUserId());

            return Ok(cart);
        }
    }
}