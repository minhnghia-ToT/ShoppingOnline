using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.DTOs.Chart_dto;
using ShoppingOnline.Data;
using ShoppingOnline.Models;

public interface IDashboardService
{
    Task<List<WeeklySalesDto>> GetWeeklySalesAsync();
    Task<DashboardOverviewDto> GetDashboardOverviewAsync();
}

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WeeklySalesDto>> GetWeeklySalesAsync()
    {
        var today = DateTime.UtcNow.Date;

        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
        var endOfWeek = startOfWeek.AddDays(7);

        // B1: Lấy data từ DB trước
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == "Delivered"
                     && o.CreatedAt >= startOfWeek
                     && o.CreatedAt < endOfWeek)
            .ToListAsync();

        // B2: Xử lý GroupBy trong memory
        var grouped = orders
            .SelectMany(o => o.OrderItems, (order, item) => new
            {
                order.CreatedAt,
                item.Quantity
            })
            .GroupBy(x => x.CreatedAt.DayOfWeek)
            .Select(g => new
            {
                DayOfWeek = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        var result = new List<WeeklySalesDto>();

        var days = new[]
        {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    };

        foreach (var day in days)
        {
            var data = grouped.FirstOrDefault(x => x.DayOfWeek == day);

            result.Add(new WeeklySalesDto
            {
                Day = day.ToString().Substring(0, 3),
                TotalQuantity = data?.TotalQuantity ?? 0
            });
        }

        return result;
    }
    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
    {
        var now = DateTime.UtcNow;

        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var last30Days = now.AddDays(-30);

        // 1️⃣ Total Products
        var totalProducts = await _context.Products.CountAsync();

        // 2️⃣ Products Sold This Month
        var productsSoldThisMonth = await _context.OrderItems
            .Where(oi => oi.Order.Status == "Delivered"
                      && oi.Order.CreatedAt >= startOfMonth)
            .SumAsync(oi => (int?)oi.Quantity) ?? 0;

        // 3️⃣ Revenue Last 30 Days
        var revenueLast30Days = await _context.Orders
            .Where(o => o.Status == "Delivered"
                     && o.CreatedAt >= last30Days)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        // 4️⃣ Active Customers (có hơn 1 đơn completed)
        var activeCustomers = await _context.Orders
            .Where(o => o.Status == "Delivered")
            .GroupBy(o => o.UserId)
            .Where(g => g.Count() > 1)
            .CountAsync();

        return new DashboardOverviewDto
        {
            TotalProducts = totalProducts,
            ProductsSoldThisMonth = productsSoldThisMonth,
            RevenueLast30Days = revenueLast30Days,
            ActiveCustomers = activeCustomers
        };
    }
}