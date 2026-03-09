using ShoppingOnline.DTOs.Orders;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> Checkout(int userId, CheckoutOrderDTO dto);

        Task<List<OrderResponseDTO>> GetMyOrders(int userId);

        Task<OrderResponseDTO?> GetOrderById(int userId, int orderId);

        Task<bool> CancelOrder(int userId, int orderId);
    }
}