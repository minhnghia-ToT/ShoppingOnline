namespace OnlineShopping.Models.DTOs.Chart_dto
{
    public class DashboardOverviewDto
    {
        public int TotalProducts { get; set; }
        public int ProductsSoldThisMonth { get; set; }
        public decimal RevenueLast30Days { get; set; }
        public int ActiveCustomers { get; set; }
    }
}
