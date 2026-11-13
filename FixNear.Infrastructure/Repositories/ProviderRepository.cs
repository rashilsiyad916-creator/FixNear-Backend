using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly FixNearDbContext _context;

        public ProviderRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<List<Provider>> GetProvidersAsync(string? category, string? area)
        {
            var query = _context.Providers
                .Include(p => p.Services).ThenInclude(s => s.Category)
                .Include(p => p.Reviews)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                query = query.Where(p =>
                    p.Services.Any(s => s.IsActive && s.Category.Name == cat));
            }

            if (!string.IsNullOrWhiteSpace(area))
            {
                var areaTerm = area.Trim();
                query = query.Where(p =>
                    p.ServiceArea.Contains(areaTerm) || p.Address.Contains(areaTerm));
            }

            return await query.OrderBy(p => p.ProviderId).ToListAsync();
        }

        public async Task<Provider?> GetByIdAsync(int id)
        {
            return await _context.Providers
                .Include(p => p.Services).ThenInclude(s => s.Category)
                .Include(p => p.Reviews)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProviderId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Providers.AnyAsync(p => p.ProviderId == id);
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

        public async Task UpdateImageAsync(int providerId, string imageUrl)
        {
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.ProviderId == providerId);

            if (provider != null)
            {
                provider.Image = imageUrl;
                await _context.SaveChangesAsync();
            }
        }
    }
}
