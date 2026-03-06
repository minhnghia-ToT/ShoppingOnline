using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.DTOs;
using ShoppingOnline.Data;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(CreateNotificationDto dto)
        {
            // Assuming UserRoles is a navigation property and Role.Name is the role name
            var admins = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"))
                .ToListAsync();

            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    UserId = admin.Id
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDto>> GetAdminNotificationsAsync(int adminId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == adminId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}