using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Categories;

namespace BusinessLogicLayer.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => _mapper.Map<CategoryDto>(c));
        }

        public async Task<CategoryDto?> GetByIdAsync(int categoryId)
        {
            var entity = await _repo.GetByIdAsync(categoryId);
            return entity == null ? null : _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<bool> UpdateAsync(CategoryDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.CategoryId);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            var entity = await _repo.GetByIdAsync(categoryId);
            if (entity == null) return false;
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
