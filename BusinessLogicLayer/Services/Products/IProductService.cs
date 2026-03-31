using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Products
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int productId);
        Task<ProductDto> CreateAsync(ProductDto dto);
        Task<bool> UpdateAsync(ProductDto dto);
        Task<bool> DeleteAsync(int productId);

        Task<PaginatedList<ProductDto>> GetPagedAsync(int pageIndex, int pageSize);
        Task<PaginatedList<ProductDto>> SearchByNameAsync(string name, int pageIndex, int pageSize);
        Task<PaginatedList<ProductDto>> SortByPriceAsync(bool ascending, int pageIndex, int pageSize);
        Task<PaginatedList<ProductDto>> SortByBestSellerAsync(int pageIndex, int pageSize);

        Task<IEnumerable<ProductDto>> GetPopularRandomAsync(int count);
        Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count);

        Task<IEnumerable<ProductDto>> GetBestSellersAsync(int count);
    }
}
