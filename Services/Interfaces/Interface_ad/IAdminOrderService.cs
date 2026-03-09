using ShoppingOnline.Models.DTOs.Order_dto_ad;

namespace ShoppingOnline.Services.Interfaces.Interface_ad
{
    public interface IAdminOrderService
    {
        Task<List<OrderAdminDTO>> GetAllOrders();

        Task<OrderDetailAdminDTO?> GetOrderById(int id);

        Task<bool> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO dto);
    }
}