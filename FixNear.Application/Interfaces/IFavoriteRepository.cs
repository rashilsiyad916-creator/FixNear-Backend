using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<List<int>> GetProviderIdsAsync(int userId);
        Task<bool> ExistsAsync(int userId, int providerId);
        Task AddAsync(Favorite favorite);
        void Remove(Favorite favorite);
        Task SaveChangesAsync();
    }
}
