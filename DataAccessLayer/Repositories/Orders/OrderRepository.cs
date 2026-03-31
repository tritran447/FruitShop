using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FruitShopContext _context;
        public OrderRepository(FruitShopContext context)
            => _context = context;

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _context.Orders
                     .Include(o => o.OrderDetails)
                       .ThenInclude(d => d.Product)
                     .ToListAsync();

        public async Task<Order?> GetByIdAsync(int orderId)
            => await _context.Orders
                             .Include(o => o.OrderDetails)
                               .ThenInclude(d => d.Product)
                             .FirstOrDefaultAsync(o => o.OrderId == orderId);
        public async Task<Order?> GetByCodeAsync(string orderCode)
            => await _context.Orders
                             .Include(o => o.OrderDetails)
                               .ThenInclude(d => d.Product)
                             .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

        public async Task AddAsync(Order order)
            => await _context.Orders.AddAsync(order);

        public void Update(Order order)
            => _context.Orders.Update(order);

        public void Remove(Order order)
            => _context.Orders.Remove(order);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<IEnumerable<Order>> GetCompletedOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Completed")
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
