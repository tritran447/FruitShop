using BusinessLogicLayer.Services.Admin;
using DataAccessLayer.Repositories.Categories;
using DataAccessLayer.Repositories.Customers;
using DataAccessLayer.Repositories.OrderDetails;
using DataAccessLayer.Repositories.Orders;
using DataAccessLayer.Repositories.Products;
using DataAccessLayer.Repositories.UserOtps;

namespace FruitShop.api.Configuration
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            services.AddScoped<IUserOtpRepository, UserOtpRepository>();

            return services;
        }
    }
}
