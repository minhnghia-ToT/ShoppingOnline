using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShoppingOnline.DTOs.Cart;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // GET api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var cart = await _cartService.GetCart(userId);

            return Ok(cart);
        }

        // POST api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDTO dto)
        {
            var userId = GetUserId();

            await _cartService.AddToCart(userId, dto);

            return Ok(new { message = "Added to cart successfully" });
        }

        // PUT api/cart/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart(UpdateCartDTO dto)
        {
            var userId = GetUserId();

            await _cartService.UpdateCart(userId, dto);

            return Ok(new { message = "Cart updated" });
        }

        // DELETE api/cart/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userId = GetUserId();

            await _cartService.RemoveItem(userId, productId);

            return Ok(new { message = "Item removed" });
        }
    }
}