using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.Orders;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDTO> Checkout(int userId, CheckoutOrderDTO dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = 0;

            foreach (var item in cart.CartItems)
            {
                var product = item.Product;

                
                if (product.Status != "Active")
                    throw new Exception($"Product {product.Name} is not available");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Product {product.Name} not enough stock");

                var price = product.DiscountPrice ?? product.Price;

                total += price * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                var product = item.Product;

                var price = product.DiscountPrice ?? product.Price;

                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = price
                });

                product.StockQuantity -= item.Quantity;
            }

            _context.OrderItems.AddRange(orderItems);

            var payment = new Payment
            {
                OrderId = order.Id,
                Method = dto.PaymentMethod,
                Status = "Pending",
                Amount = total
            };

            _context.Payments.Add(payment);

            var history = new OrderHistory
            {
                OrderId = order.Id,
                Status = "Pending"
            };

            _context.OrderHistories.Add(history);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return await GetOrderById(userId, order.Id);
        }

        public async Task<List<OrderResponseDTO>> GetMyOrders(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .Select(o => new OrderResponseDTO
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,

                    PaymentMethod = _context.Payments
                        .Where(p => p.OrderId == o.Id)
                        .Select(p => p.Method)
                        .FirstOrDefault(),

                    PaymentStatus = _context.Payments
                        .Where(p => p.OrderId == o.Id)
                        .Select(p => p.Status)
                        .FirstOrDefault(),

                    Items = o.OrderItems.Select(i => new OrderItemResponseDTO
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price,

                        ImageUrl = i.Product.Images
                            .OrderByDescending(img => img.IsMain)
                            .Select(img => img.ImageUrl)
                            .FirstOrDefault()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderResponseDTO?> GetOrderById(int userId, int orderId)
        {
            return await _context.Orders
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .Select(o => new OrderResponseDTO
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,

                    PaymentMethod = _context.Payments
                        .Where(p => p.OrderId == o.Id)
                        .Select(p => p.Method)
                        .FirstOrDefault(),

                    PaymentStatus = _context.Payments
                        .Where(p => p.OrderId == o.Id)
                        .Select(p => p.Status)
                        .FirstOrDefault(),

                    Items = o.OrderItems.Select(i => new OrderItemResponseDTO
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price,

                        ImageUrl = i.Product.Images
                            .OrderByDescending(img => img.IsMain)
                            .Select(img => img.ImageUrl)
                            .FirstOrDefault()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CancelOrder(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return false;

            if (order.Status != "Pending" && order.Status != "Confirmed")
                throw new Exception("Order cannot be cancelled");

            order.Status = "Cancelled";

            foreach (var item in order.OrderItems)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            var history = new OrderHistory
            {
                OrderId = order.Id,
                Status = "Cancelled"
            };

            _context.OrderHistories.Add(history);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}