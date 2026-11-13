using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly FixNearDbContext _context;

        public BookingRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetByCustomerAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.ProviderService).ThenInclude(s => s.Provider)
                .Include(b => b.Reviews)
                .AsNoTracking()
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CompletePastBookingsAsync(int customerId, DateTime beforeDate)
        {
            // Pending/Accepted bookings whose scheduled day is over are complete.
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId
                            && b.ScheduledAt < beforeDate
                            && (b.Status == "Pending" || b.Status == "Accepted"))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(b => b.Status, "Completed"));
        }

        public async Task<ProviderService?> GetActiveServiceByProviderAsync(int providerId, int? providerServiceId = null)
        {
            var query = _context.ProviderServices
                .Include(s => s.Provider)
                .Where(s => s.ProviderId == providerId && s.IsActive);

            if (providerServiceId.HasValue)
                query = query.Where(s => s.ProviderServiceId == providerServiceId.Value);

            return await query
                .OrderBy(s => s.ProviderServiceId)
                .FirstOrDefaultAsync();
        }

        public async Task<Booking?> GetByIdForCustomerAsync(int bookingId, int customerId)
        {
            return await _context.Bookings
                .Include(b => b.ProviderService).ThenInclude(s => s.Provider)
                .Include(b => b.Customer)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
