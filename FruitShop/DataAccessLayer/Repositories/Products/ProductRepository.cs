using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly FruitShopContext _context;

        public ProductRepository(FruitShopContext context)
        {
            _context = context;
        }

        // Basic CRUD
        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.OrderDetails)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.Status == true);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Pagination, search, sort
        public async Task<PaginatedList<Product>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .Where(p => p.Status == true)
                                .OrderBy(p => p.ProductId);
            return await PaginatedList<Product>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<Product>> SearchByNameAsync(string name, int pageIndex, int pageSize)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .Where(p => p.ProductName.Contains(name) && p.Status == true);
            return await PaginatedList<Product>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<Product>> SortByPriceAsync(bool ascending, int pageIndex, int pageSize)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .Where(p => p.Status == true);

            query = ascending
                ? query.OrderBy(p => p.Price)
                : query.OrderByDescending(p => p.Price);

            return await PaginatedList<Product>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<Product>> SortByBestSellerAsync(int pageIndex, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.OrderDetails)
                .AsNoTracking()
                .Where(p => p.Status == true)
                .Select(p => new
                {
                    Product = p,
                    Sold = p.OrderDetails.Sum(od => od.Quantity) ?? 0
                })
                .OrderByDescending(x => x.Sold)
                .Select(x => x.Product);

            return await PaginatedList<Product>.CreateAsync(query, pageIndex, pageSize);
        }

        // Popular random products
        public async Task<IEnumerable<Product>> GetPopularRandomAsync(int count)
        {
            var topProducts = await _context.Products
                .Include(p => p.OrderDetails)
                .AsNoTracking()
                .Where(p => p.Status == true)
                .Select(p => new
                {
                    Product = p,
                    Sold = p.OrderDetails.Sum(od => od.Quantity) ?? 0
                })
                .OrderByDescending(x => x.Sold)
                .Take(20)
                .Select(x => x.Product)
                .ToListAsync();

            return topProducts.OrderBy(_ => Guid.NewGuid()).Take(count);
        }

        // Related products: same category, excluding self
        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count)
        {
            // Lấy category của sản phẩm
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null || product.CategoryId == null)
                return Array.Empty<Product>();

            var query = _context.Products
                 .Include(p => p.Category)
                 .AsNoTracking()
                 .Where(p => p.CategoryId == product.CategoryId && p.ProductId != productId && p.Status == true)
                 .OrderBy(_ => Guid.NewGuid())
                 .Take(count);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithTotalSoldAsync()
        {
            return await _context.Products
                .Include(p => p.OrderDetails)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSellersAsync(int count)
        {
            var bestSellers = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Product != null)
                .GroupBy(od => od.Product) 
                .Select(g => new
                {
                    Product = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(count)
                .Select(g => g.Product)
                .AsNoTracking()
                .ToListAsync();

            return bestSellers;
        }
    }
}
