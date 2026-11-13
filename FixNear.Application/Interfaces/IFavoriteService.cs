namespace FixNear.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<IReadOnlyList<int>> GetFavoritesAsync(int userId);
        Task AddFavoriteAsync(int userId, int providerId);
        Task RemoveFavoriteAsync(int userId, int providerId);
    }
}
