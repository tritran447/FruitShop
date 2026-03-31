using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ReverseMap();

            CreateMap<LoginDto, Customer>()
                .ReverseMap();

            CreateMap<RegisterDto, Customer>()
                .ReverseMap();

            CreateMap<ProductDto, Product>()
                .ReverseMap();

            CreateMap<CategoryDto, Category>()
                .ReverseMap();


            CreateMap<OrderDetail, OrderDetailDto>()
                .ReverseMap();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderDetails,
                           opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap();

            CreateMap<Order, OrderHistoryDto>()
                .ForMember(d => d.Items,
                     opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderHistoryItemDto>()
              .ForMember(d => d.ProductName,
                         opt => opt.MapFrom(src => src.Product!.ProductName));

            CreateMap<Product, ProductSaleDto>()
            .ForMember(dest => dest.TotalQuantitySold,
                       opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.Quantity ?? 0)));

        }
    }
}
