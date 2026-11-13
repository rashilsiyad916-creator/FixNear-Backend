using FixNear.Application.DTOs.Category;

namespace FixNear.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }
}
