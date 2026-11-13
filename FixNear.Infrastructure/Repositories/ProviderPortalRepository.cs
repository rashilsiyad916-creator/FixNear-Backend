using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class ProviderPortalRepository : IProviderPortalRepository
    {
        private readonly FixNearDbContext _context;

        public ProviderPortalRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<Provider?> GetByUserIdAsync(int userId)
        {
            return await _context.Providers
                .Include(p => p.User)
                .Include(p => p.Services).ThenInclude(s => s.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task AddProviderAsync(Provider provider)
        {
            await _context.Providers.AddAsync(provider);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProviderService>> GetServicesAsync(int providerId)
        {
            return await _context.ProviderServices
                .Include(s => s.Category)
                .Where(s => s.ProviderId == providerId)
                .OrderBy(s => s.ProviderServiceId)
                .ToListAsync();
        }

        public async Task<ProviderService?> GetServiceAsync(int providerId, int serviceId)
        {
            return await _context.ProviderServices
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ProviderId == providerId && s.ProviderServiceId == serviceId);
        }

        public async Task AddServiceAsync(ProviderService service)
        {
            await _context.ProviderServices.AddAsync(service);
        }

        public async Task<int> CountBookingsForServiceAsync(int serviceId)
        {
            return await _context.Bookings.CountAsync(b => b.ProviderServiceId == serviceId);
        }

        public void RemoveService(ProviderService service)
        {
            _context.ProviderServices.Remove(service);
        }

        public async Task<List<Booking>> GetBookingsAsync(int providerId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.ProviderService)
                .Where(b => b.ProviderService.ProviderId == providerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingAsync(int providerId, int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.ProviderService)
                .FirstOrDefaultAsync(b =>
                    b.BookingId == bookingId &&
                    b.ProviderService.ProviderId == providerId);
        }

        public async Task<int> CompletePastBookingsAsync(int providerId, DateTime beforeDate)
        {
            return await _context.Bookings
                .Where(b => b.ProviderService.ProviderId == providerId
                            && b.ScheduledAt < beforeDate
                            && (b.Status == "Pending" || b.Status == "Accepted"))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(b => b.Status, "Completed"));
        }

        public async Task<List<Review>> GetReviewsAsync(int providerId)
        {
            return await _context.Reviews
                .Where(r => r.ProviderId == providerId)
                .Include(r => r.Customer)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryAsync(int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.IsActive);
        }
    }
}
