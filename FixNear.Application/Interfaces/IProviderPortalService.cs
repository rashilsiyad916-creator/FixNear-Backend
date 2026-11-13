using FixNear.Application.DTOs.Category;
using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;

namespace FixNear.Application.Interfaces
{
    public interface IProviderPortalService
    {
        Task<ProviderProfileDto?> GetProfileAsync(int userId);
        Task<ProviderProfileDto> SaveProfileAsync(int userId, ProviderProfileRequest request);
        Task<ImageUploadResponse> UploadPhotoAsync(int userId, Stream content, string fileName);
        Task<ImageUploadResponse> UploadServicePhotoAsync(int userId, Stream content, string fileName);

        Task<List<ProviderServiceDto>> GetServicesAsync(int userId);
        Task<ProviderServiceDto> AddServiceAsync(int userId, ProviderServiceRequest request);
        Task<ProviderServiceDto?> UpdateServiceAsync(int userId, int serviceId, ProviderServiceRequest request);
        Task<bool> DeleteServiceAsync(int userId, int serviceId);

        Task<List<ProviderBookingDto>> GetBookingsAsync(int userId);
        Task<ProviderBookingDto?> UpdateBookingStatusAsync(int userId, int bookingId, string status);
        Task<bool> SetAvailabilityAsync(int userId, bool available);
        Task<ProviderStatsDto?> GetStatsAsync(int userId);
        Task<List<ReviewDto>> GetReviewsAsync(int userId);

        Task<List<CategoryRequestDto>> GetCategoryRequestsAsync(int userId);
        Task<CategoryRequestDto> SubmitCategoryRequestAsync(int userId, CreateCategoryRequest request);
    }
}
