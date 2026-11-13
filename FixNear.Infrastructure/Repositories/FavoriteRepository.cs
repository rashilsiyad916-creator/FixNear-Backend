using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly FixNearDbContext _context;

        public FavoriteRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetProviderIdsAsync(int userId)
        {
            return await _context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.ProviderId)
                .Select(f => f.ProviderId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, int providerId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.ProviderId == providerId);
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public void Remove(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
