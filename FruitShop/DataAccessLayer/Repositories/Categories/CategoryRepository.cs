using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FruitShopContext _context;
        public CategoryRepository(FruitShopContext context) => _context = context;
        public async Task<IEnumerable<Category>> GetAllAsync() =>
            await _context.Categories.AsNoTracking().ToListAsync();
        public async Task<Category?> GetByIdAsync(int categoryId) =>
            await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        public async Task AddAsync(Category category) => await _context.Categories.AddAsync(category);
        public void Update(Category category) => _context.Categories.Update(category);
        public void Delete(Category category) => _context.Categories.Remove(category);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
