using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface IProviderPortalRepository
    {
        Task<Provider?> GetByUserIdAsync(int userId);
        Task<User?> GetUserAsync(int userId);
        Task AddProviderAsync(Provider provider);
        Task SaveChangesAsync();

        Task<List<ProviderService>> GetServicesAsync(int providerId);
        Task<ProviderService?> GetServiceAsync(int providerId, int serviceId);
        Task AddServiceAsync(ProviderService service);
        Task<int> CountBookingsForServiceAsync(int serviceId);
        void RemoveService(ProviderService service);

        Task<List<Booking>> GetBookingsAsync(int providerId);
        Task<Booking?> GetBookingAsync(int providerId, int bookingId);
        Task<int> CompletePastBookingsAsync(int providerId, DateTime beforeDate);
        Task<List<Review>> GetReviewsAsync(int providerId);

        Task<Category?> GetCategoryAsync(int categoryId);
    }
}
