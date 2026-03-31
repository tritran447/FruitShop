using BusinessLogicLayer.Dtos;

namespace BusinessLogicLayer.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int categoryId);
        Task<CategoryDto> CreateAsync(CategoryDto dto);
        Task<bool> UpdateAsync(CategoryDto dto);
        Task<bool> DeleteAsync(int categoryId);
    }
}
