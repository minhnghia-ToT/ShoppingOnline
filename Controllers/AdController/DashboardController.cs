using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
/*using ShoppingOnline.Services.Interfaces;*/
using OnlineShopping.Models.DTOs.Chart_dto;
using OnlineShopping.Services.Interfaces.Interface_ad;

namespace ShoppingOnline.Controllers.Admin
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("weekly-sales")]
        public async Task<IActionResult> GetWeeklySales()
        {
            var result = await _dashboardService.GetWeeklySalesAsync();
            return Ok(result);
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var result = await _dashboardService.GetDashboardOverviewAsync();
            return Ok(result);
        }
    }
}