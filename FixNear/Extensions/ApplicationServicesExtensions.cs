using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using FixNear.Infrastructure.Data;
using FixNear.Infrastructure.Repositories;
using FixNear.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<FixNearDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("FixNearDB"),
                    sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IProviderPortalRepository, ProviderPortalRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<ICategoryRequestRepository, CategoryRequestRepository>();

            services.AddSingleton<IPhotoStorageService, CloudinaryPhotoStorageService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProviderService, FixNear.Infrastructure.Services.ProviderService>();
            services.AddScoped<IProviderPortalService, ProviderPortalService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<ICategoryRequestService, CategoryRequestService>();

            services.AddCors(options => options.AddPolicy("DevCors", policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            return services;
        }
    }
}
