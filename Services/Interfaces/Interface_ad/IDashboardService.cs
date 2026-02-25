using OnlineShopping.Models.DTOs.Chart_dto;

namespace OnlineShopping.Services.Interfaces.Interface_ad
{
    public interface IDashboardService
    {
        Task<List<WeeklySalesDto>> GetWeeklySalesAsync();
        Task<DashboardOverviewDto> GetDashboardOverviewAsync();
    }

}
