using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.OrderDetails
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly FruitShopContext _context;
        public OrderDetailRepository(FruitShopContext context)
            => _context = context;

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
            => await _context.OrderDetails
                             .Where(d => d.OrderId == orderId)
                             .Include(d => d.Product)
                             .ToListAsync();

        public async Task AddAsync(OrderDetail detail)
            => await _context.OrderDetails.AddAsync(detail);

        public void Remove(OrderDetail detail)
            => _context.OrderDetails.Remove(detail);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
