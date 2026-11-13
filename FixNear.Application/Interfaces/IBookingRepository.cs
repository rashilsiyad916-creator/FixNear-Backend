using FixNear.Domain.Entities;

namespace FixNear.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetByCustomerAsync(int customerId);
        Task<int> CompletePastBookingsAsync(int customerId, DateTime beforeDate);
        Task<ProviderService?> GetActiveServiceByProviderAsync(int providerId, int? providerServiceId = null);
        Task<Booking?> GetByIdForCustomerAsync(int bookingId, int customerId);
        Task AddAsync(Booking booking);
        Task AddReviewAsync(Review review);
        Task SaveChangesAsync();
    }
}
