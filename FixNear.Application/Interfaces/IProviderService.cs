using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;

namespace FixNear.Application.Interfaces
{
    public interface IProviderService
    {
        Task<IEnumerable<ProviderDto>> GetAllAsync(string? category, string? area);
        Task<ProviderDto?> GetByIdAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ImageUploadResponse> ChangePhotoAsync(int providerId, Stream content, string fileName);
    }
}
