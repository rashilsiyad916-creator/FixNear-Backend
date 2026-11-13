using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetActiveAsync();
    }
}
