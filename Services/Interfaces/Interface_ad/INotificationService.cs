using OnlineShopping.Models.DTOs;


namespace ShoppingOnline.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(CreateNotificationDto dto);

        Task<List<NotificationDto>> GetAdminNotificationsAsync(int adminId);

        Task MarkAsReadAsync(int notificationId);
    }
}