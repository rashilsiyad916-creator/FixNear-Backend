using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface IProviderRepository
    {
        Task<List<Provider>> GetProvidersAsync(string? category, string? area);
        Task<Provider?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Review>> GetReviewsAsync(int providerId);
        Task UpdateImageAsync(int providerId, string imageUrl);
    }
}
