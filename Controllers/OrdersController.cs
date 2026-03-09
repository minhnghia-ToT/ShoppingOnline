using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Checkout Order
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutOrderDTO dto)
        {
            var userId = GetUserId();

            var order = await _orderService.Checkout(userId, dto);

            return Ok(order);
        }

        // Get all orders of current user
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
                return NotFound("Order not found");

            return Ok(order);
        }

        // Cancel order
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserId();

            var result = await _orderService.CancelOrder(userId, id);

            if (!result)
                return NotFound("Order not found");

            return Ok(new
            {
                message = "Order cancelled successfully"
            });
        }
    }
}