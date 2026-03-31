using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos;

namespace BusinessLogicLayer.Services.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int orderId);
        Task<OrderDto> CreateAsync(OrderDto dto);
        Task UpdateAsync(OrderDto dto);
        Task DeleteAsync(int orderId);
        Task<OrderDto> CreateAsync(CreateOrderDto create);

        Task<IEnumerable<OrderHistoryDto>> GetHistoryByCustomerAsync(int customerId);
    }
}
