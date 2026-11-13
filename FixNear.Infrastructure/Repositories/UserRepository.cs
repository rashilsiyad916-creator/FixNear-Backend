using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FixNearDbContext _context;

        public UserRepository(FixNearDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLower();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == normalized);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var normalized = email.Trim().ToLower();
            return await _context.Users.AnyAsync(u => u.Email == normalized);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
