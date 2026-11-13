using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FixNearDbContext _context;

        public CategoryRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetActiveAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryId)
                .ToListAsync();
        }
    }
}
