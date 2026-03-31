using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Products;

namespace BusinessLogicLayer.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProductDto?> GetByIdAsync(int productId)
        {
            var e = await _repo.GetByIdAsync(productId);
            return e == null ? null : _mapper.Map<ProductDto>(e);
        }

        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            var e = _mapper.Map<Product>(dto);
            await _repo.AddAsync(e);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProductDto>(e);
        }

        public async Task<bool> UpdateAsync(ProductDto dto)
        {
            var e = await _repo.GetByIdAsync(dto.ProductId);
            if (e == null) return false;
            _mapper.Map(dto, e);
            _repo.Update(e);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var e = await _repo.GetByIdAsync(productId);
            if (e == null) return false;
            _repo.Delete(e);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedList<ProductDto>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var list = await _repo.GetPagedAsync(pageIndex, pageSize);
            var dtos = list.Select(p => _mapper.Map<ProductDto>(p)).ToList();
            return new PaginatedList<ProductDto>(dtos, list.TotalCount, list.PageIndex, pageSize);
        }

        public async Task<PaginatedList<ProductDto>> SearchByNameAsync(string name, int pageIndex, int pageSize)
        {
            var list = await _repo.SearchByNameAsync(name, pageIndex, pageSize);
            var dtos = list.Select(p => _mapper.Map<ProductDto>(p)).ToList();
            return new PaginatedList<ProductDto>(dtos, list.TotalCount, list.PageIndex, pageSize);
        }

        public async Task<PaginatedList<ProductDto>> SortByPriceAsync(bool ascending, int pageIndex, int pageSize)
        {
            var list = await _repo.SortByPriceAsync(ascending, pageIndex, pageSize);
            var dtos = list.Select(p => _mapper.Map<ProductDto>(p)).ToList();
            return new PaginatedList<ProductDto>(dtos, list.TotalCount, list.PageIndex, pageSize);
        }

        public async Task<PaginatedList<ProductDto>> SortByBestSellerAsync(int pageIndex, int pageSize)
        {
            var list = await _repo.SortByBestSellerAsync(pageIndex, pageSize);
            var dtos = list.Select(p => _mapper.Map<ProductDto>(p)).ToList();
            return new PaginatedList<ProductDto>(dtos, list.TotalCount, list.PageIndex, pageSize);
        }

        public async Task<IEnumerable<ProductDto>> GetPopularRandomAsync(int count)
        {
            var list = await _repo.GetPopularRandomAsync(count);
            return list.Select(p => _mapper.Map<ProductDto>(p));
        }

        public async Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count)
        {
            var list = await _repo.GetRelatedProductsAsync(productId, count);
            return list.Select(p => _mapper.Map<ProductDto>(p));
        }
        public async Task<IEnumerable<ProductDto>> GetBestSellersAsync(int count)
        {
            var products = await _repo.GetBestSellersAsync(count);
            return products.Select(p => _mapper.Map<ProductDto>(p));
        }

    }
}
