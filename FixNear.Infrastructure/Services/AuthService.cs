using FixNear.Application.DTOs.Auth;
using FixNear.Application.Exceptions;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FixNear.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher<User> _hasher;
        private readonly ITokenService _tokens;

        public AuthService(IUserRepository users, IPasswordHasher<User> hasher, ITokenService tokens)
        {
            _users = users;
            _hasher = hasher;
            _tokens = tokens;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new BadRequestException("Full name is required");
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
                throw new BadRequestException("A valid email is required");
            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
                throw new BadRequestException("Password must be at least 8 characters");

            var role = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role.Trim();
            if (role != "Customer" && role != "Provider")
                throw new BadRequestException("Role must be Customer or Provider");

            var email = request.Email.Trim().ToLower();
            if (await _users.EmailExistsAsync(email))
                throw new ConflictException("An account with this email already exists");

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _users.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                throw new UnauthorizedException("Invalid email or password");

            return BuildAuthResponse(user);
        }

        public async Task<UserInfoDto?> GetMeAsync(int userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserInfoDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        private AuthResponse BuildAuthResponse(User user)
        {
            var (token, expiresAt) = _tokens.GenerateToken(user);
            return new AuthResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
