using OnlineShopping.Models.DTOs;
using ShoppingOnline.DTOs.Orders;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IOrderService
    {
        // Checkout từ giỏ hàng
        Task<OrderResponseDTO> CheckoutCart(int userId, CheckoutOrderDTO dto);

        // Mua ngay (không qua cart)
        Task<OrderResponseDTO> BuyNow(int userId, BuyNowDTO dto);

        // Lấy danh sách đơn của user
        Task<List<OrderResponseDTO>> GetMyOrders(int userId);

        // Lấy chi tiết order
        Task<OrderResponseDTO?> GetOrderById(int userId, int orderId);

        // Hủy order
        Task<bool> CancelOrder(int userId, int orderId);
    }
}