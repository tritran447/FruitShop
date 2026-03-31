using BusinessLogicLayer.Dtos;

namespace BusinessLogicLayer.Services.Admin
{
    public interface IAdminDashboardService
    {
        Task<IEnumerable<ProductSaleDto>> GetTopSellingProductsAsync(int top = 5);
        Task<RevenueDto> GetRevenueStatsAsync();
        Task<int> CountActiveCustomersAsync();
    }
}
