using FixNear.Application.Exceptions;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;

namespace FixNear.Infrastructure.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favorites;
        private readonly IProviderRepository _providers;

        public FavoriteService(IFavoriteRepository favorites, IProviderRepository providers)
        {
            _favorites = favorites;
            _providers = providers;
        }

        public async Task<IReadOnlyList<int>> GetFavoritesAsync(int userId)
        {
            return await _favorites.GetProviderIdsAsync(userId);
        }

        public async Task AddFavoriteAsync(int userId, int providerId)
        {
            if (!await _providers.ExistsAsync(providerId))
                throw new NotFoundException("Provider not found");

            if (await _favorites.ExistsAsync(userId, providerId))
                return;

            await _favorites.AddAsync(new Favorite
            {
                UserId = userId,
                ProviderId = providerId
            });
            await _favorites.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int userId, int providerId)
        {
            if (!await _favorites.ExistsAsync(userId, providerId))
                return;

            _favorites.Remove(new Favorite { UserId = userId, ProviderId = providerId });
            await _favorites.SaveChangesAsync();
        }
    }
}
