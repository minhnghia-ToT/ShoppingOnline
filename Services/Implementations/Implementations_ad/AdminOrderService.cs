using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Models;
using ShoppingOnline.Models.DTOs.Order_dto_ad;
using ShoppingOnline.Services.Interfaces.Interface_ad;

namespace ShoppingOnline.Services.Implementations.Implementations_ad
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly ApplicationDbContext _context;

        public AdminOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderAdminDTO>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .Select(o => new OrderAdminDTO
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.Payment.Method,
                    PaymentStatus = o.Payment.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<OrderDetailAdminDTO?> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDetailAdminDTO
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.Payment.Method,
                PaymentStatus = order.Payment.Status,
                CreatedAt = order.CreatedAt
            };
        }
        
        public async Task<bool> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.OrderHistories)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            string currentStatus = order.Status;
            string newStatus = dto.Status;

            if (!IsValidStatusTransition(currentStatus, newStatus))
                throw new Exception("Invalid status transition");

            order.Status = newStatus;

            if (order.Payment.Method == "COD" && newStatus == "Delivered")
            {
                order.Payment.Status = "Paid";
            }

            var history = new OrderHistory
            {
                OrderId = order.Id,
                Status = newStatus,
                ChangedAt = DateTime.UtcNow
            };

            _context.OrderHistories.Add(history);

            await _context.SaveChangesAsync();

            return true;
        }

        private bool IsValidStatusTransition(string current, string next)
        {
            return (current == "Pending" && next == "Confirmed") ||
                   (current == "Confirmed" && next == "Processing") ||
                   (current == "Processing" && next == "Shipping") ||
                   (current == "Shipping" && next == "Delivered");
        }
    }
}