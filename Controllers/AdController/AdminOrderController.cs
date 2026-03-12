using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Models.DTOs.Order_dto_ad;
using ShoppingOnline.Services.Interfaces.Interface_ad;

namespace ShoppingOnline.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IAdminOrderService _orderService;

        public AdminOrdersController(IAdminOrderService orderService)
        {
            _orderService = orderService;
        }

        // Get all orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        // Get order detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);

            if (order == null)
                return NotFound(new
                {
                    message = "Order not found"
                });

            return Ok(order);
        }

        // Update order status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDTO dto)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatus(id, dto);

                if (!result)
                    return NotFound(new
                    {
                        message = "Order not found"
                    });

                return Ok(new
                {
                    message = "Order status updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}