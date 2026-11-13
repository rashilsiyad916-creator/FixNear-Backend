using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface ICategoryRequestRepository
    {
        Task<Provider?> GetProviderByUserIdAsync(int userId);
        Task<List<CategoryRequest>> GetForProviderAsync(int providerId);
        Task<List<CategoryRequest>> GetByStatusAsync(string? status);
        Task<CategoryRequest?> GetByIdAsync(int id);

        Task<bool> HasPendingRequestAsync(int providerId, string name);
        Task<Category?> FindCategoryByNameAsync(string name);
        Task AddRequestAsync(CategoryRequest request);
        Task AddCategoryAsync(Category category);
        Task SaveChangesAsync();
    }
}
