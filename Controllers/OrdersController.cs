using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models.DTOs;
using ShoppingOnline.DTOs.Orders;
using ShoppingOnline.Services.Interfaces;
using System.Security.Claims;

namespace ShoppingOnline.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        // Checkout from cart
        [HttpPost("checkout-cart")]
        public async Task<IActionResult> CheckoutCart([FromBody] CheckoutOrderDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _orderService.CheckoutCart(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Buy now
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _orderService.BuyNow(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get all orders
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetMyOrders(userId);
            return Ok(orders);
        }

        // Get order detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = GetUserId();

            var order = await _orderService.GetOrderById(userId, id);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        // Cancel order
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = GetUserId();

                var result = await _orderService.CancelOrder(userId, id);

                if (!result)
                    return NotFound(new { message = "Order not found" });

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}