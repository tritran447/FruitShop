using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Customers;
using DataAccessLayer.Repositories.Orders;
using DataAccessLayer.Repositories.Products;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;

        public AdminDashboardService(
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
        }

        public async Task<IEnumerable<ProductSaleDto>> GetTopSellingProductsAsync(int top = 5)
        {
            var products = await _productRepo.GetProductsWithTotalSoldAsync();

            var ranked = products
                .Select(p => new ProductSaleDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    TotalQuantitySold = p.OrderDetails.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(top);

            return ranked;
        }

        public async Task<RevenueDto> GetRevenueStatsAsync()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;

            var orders = await _orderRepo.GetCompletedOrdersAsync();

            var todayRevenue = orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == today)
                .Sum(o => o.TotalAmount ?? 0);

            var monthRevenue = orders
                .Where(o => o.OrderDate.HasValue &&
                            o.OrderDate.Value.Month == now.Month &&
                            o.OrderDate.Value.Year == now.Year)
                .Sum(o => o.TotalAmount ?? 0);

            var yearRevenue = orders
                .Where(o => o.OrderDate.HasValue &&
                            o.OrderDate.Value.Year == now.Year)
                .Sum(o => o.TotalAmount ?? 0);

            return new RevenueDto
            {
                Today = todayRevenue,
                ThisMonth = monthRevenue,
                ThisYear = yearRevenue
            };
        }

        public async Task<int> CountActiveCustomersAsync()
        {
            var users = await _customerRepo.GetAllAsync();
            return users.Count(c => c.Role == "User" && c.IsVerified);
        }
    }
}
