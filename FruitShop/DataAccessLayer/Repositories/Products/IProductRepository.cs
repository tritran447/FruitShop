// DataAccessLayer/Repositories/Products/IProductRepository.cs
using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Products
{
    public interface IProductRepository
    {
        // Basic CRUD
        Task<Product?> GetByIdAsync(int productId);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
        Task<int> SaveChangesAsync();

        // Pagination, search and sorting
        Task<PaginatedList<Product>> GetPagedAsync(int pageIndex, int pageSize);
        Task<PaginatedList<Product>> SearchByNameAsync(string name, int pageIndex, int pageSize);
        Task<PaginatedList<Product>> SortByPriceAsync(bool ascending, int pageIndex, int pageSize);
        Task<PaginatedList<Product>> SortByBestSellerAsync(int pageIndex, int pageSize);

        // Popular random products
        Task<IEnumerable<Product>> GetPopularRandomAsync(int count);

        // Related products: same category, excluding self
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count);

        Task<IEnumerable<Product>> GetProductsWithTotalSoldAsync();

        Task<IEnumerable<Product>> GetBestSellersAsync(int count);


    }
}
