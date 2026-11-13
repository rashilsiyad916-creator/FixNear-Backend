using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class CategoryRequestRepository : ICategoryRequestRepository
    {
        private readonly FixNearDbContext _context;

        public CategoryRequestRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<Provider?> GetProviderByUserIdAsync(int userId)
        {
            return await _context.Providers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<CategoryRequest>> GetForProviderAsync(int providerId)
        {
            return await _context.CategoryRequests
                .AsNoTracking()
                .Include(r => r.Provider).ThenInclude(p => p.User)
                .Where(r => r.ProviderId == providerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CategoryRequest>> GetByStatusAsync(string? status)
        {
            var query = _context.CategoryRequests
                .AsNoTracking()
                .Include(r => r.Provider).ThenInclude(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalized = status.Trim();
                query = query.Where(r => r.Status == normalized);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<CategoryRequest?> GetByIdAsync(int id)
        {
            return await _context.CategoryRequests
                .Include(r => r.Provider).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.CategoryRequestId == id);
        }

        public async Task<bool> HasPendingRequestAsync(int providerId, string name)
        {
            return await _context.CategoryRequests.AnyAsync(r =>
                r.ProviderId == providerId &&
                r.Status == CategoryRequest.Pending &&
                r.Name.ToLower() == name.ToLower());
        }

        public async Task<Category?> FindCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task AddRequestAsync(CategoryRequest request)
        {
            await _context.CategoryRequests.AddAsync(request);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
