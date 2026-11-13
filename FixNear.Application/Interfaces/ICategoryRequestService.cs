using FixNear.Application.DTOs.Category;

namespace FixNear.Application.Interfaces
{
    public interface ICategoryRequestService
    {
        // Provider side
        Task<List<CategoryRequestDto>> GetForProviderAsync(int userId);
        Task<CategoryRequestDto> SubmitAsync(int userId, CreateCategoryRequest request);

        // Admin side
        Task<List<CategoryRequestDto>> GetAllAsync(string? status);
        Task<CategoryRequestDto?> ApproveAsync(int adminUserId, int requestId, string? note);
        Task<CategoryRequestDto?> RejectAsync(int adminUserId, int requestId, string? note);
    }
}
