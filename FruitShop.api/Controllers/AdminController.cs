using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FruitShop.api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboard;

        public AdminController(IAdminDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        [HttpGet("top-selling")]
        public async Task<ActionResult<IEnumerable<ProductSaleDto>>> GetTopSellingProducts()
        {
            return Ok(await _dashboard.GetTopSellingProductsAsync());
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueDto>> GetRevenue()
        {
            return Ok(await _dashboard.GetRevenueStatsAsync());
        }

        [HttpGet("active-customers")]
        public async Task<ActionResult<int>> GetActiveCustomerCount()
        {
            return Ok(await _dashboard.CountActiveCustomersAsync());
        }
    }
}
