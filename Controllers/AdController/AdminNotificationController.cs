using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Controllers.Admin
{
    [Route("api/admin/notifications")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public AdminNotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{adminId}")]
        public async Task<IActionResult> GetNotifications(int adminId)
        {
            var result = await _notificationService.GetAdminNotificationsAsync(adminId);
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }
    }
}