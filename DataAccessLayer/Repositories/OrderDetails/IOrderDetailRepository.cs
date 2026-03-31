using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.OrderDetails
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task AddAsync(OrderDetail detail);
        void Remove(OrderDetail detail);
        Task SaveChangesAsync();
    }
}
