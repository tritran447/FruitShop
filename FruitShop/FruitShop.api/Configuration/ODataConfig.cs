using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace FruitShop.Api.Configuration
{
    public static class ODataConfig
    {
        public static void ConfigureOData(this IServiceCollection services)
        {
            services.AddControllers()
                   .AddOData(options =>
                   {
                       options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
                       options.AddRouteComponents("odata", GetEdmModel());
                   });

        }
        static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new();
            builder.EntitySet<OrderDto>("Order");
            return builder.GetEdmModel();
        }
    }
}


