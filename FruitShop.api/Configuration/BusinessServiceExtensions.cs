using BusinessLogicLayer.Mapper;
using BusinessLogicLayer.Services.Admin;
using BusinessLogicLayer.Services.Authen;
using BusinessLogicLayer.Services.Categories;
using BusinessLogicLayer.Services.Customers;
using BusinessLogicLayer.Services.Orders;
using BusinessLogicLayer.Services.Products;
using BusinessLogicLayer.Services.VnPay;

namespace FruitShop.api.Configuration
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IVnPayService, VnPayService>();

            return services;
        }
    }
}
