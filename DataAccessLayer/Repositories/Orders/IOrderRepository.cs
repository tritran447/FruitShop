using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> GetByCodeAsync(string orderCode);
        Task AddAsync(Order order);
        void Update(Order order);
        void Remove(Order order);

        Task<IEnumerable<Order>> GetCompletedOrdersAsync();
        Task SaveChangesAsync();
    }
}
