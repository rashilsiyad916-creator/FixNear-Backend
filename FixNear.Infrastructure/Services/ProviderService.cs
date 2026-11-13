using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;

namespace FixNear.Infrastructure.Services
{
    public class ProviderService : IProviderService
    {
        private const string DefaultImage =
            "/assets/services/electrician-wiring-pro-v2.jpg";

        private readonly IProviderRepository _repository;
        private readonly IPhotoStorageService _photos;

        public ProviderService(IProviderRepository repository, IPhotoStorageService photos)
        {
            _repository = repository;
            _photos = photos;
        }

        public async Task<IEnumerable<ProviderDto>> GetAllAsync(string? category, string? area)
        {
            var providers = await _repository.GetProvidersAsync(category, area);
            return providers.Select(Map);
        }

        public async Task<ProviderDto?> GetByIdAsync(int id)
        {
            var provider = await _repository.GetByIdAsync(id);
            return provider == null ? null : Map(provider);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int id)
        {
            var reviews = await _repository.GetReviewsAsync(id);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.ReviewId,
                ProviderId = r.ProviderId,
                Author = r.Customer.FullName,
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                Date = r.CreatedAt.ToString("yyyy-MM-dd")
            });
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<ImageUploadResponse> ChangePhotoAsync(int providerId, Stream content, string fileName)
        {
            if (!await _repository.ExistsAsync(providerId))
                throw new Application.Exceptions.NotFoundException("Provider not found");

            var photo = await _photos.UploadFileAsync(content, fileName);
            await _repository.UpdateImageAsync(providerId, photo.Url);

            return new ImageUploadResponse
            {
                ImageUrl = photo.Url,
                PublicId = photo.PublicId,
                Format = photo.Format,
                Width = photo.Width,
                Height = photo.Height,
                Bytes = photo.Bytes,
                Folder = photo.Folder
            };
        }

        private static ProviderDto Map(Provider p)
        {
            var activeServices = p.Services.Where(s => s.IsActive).ToList();

            return new ProviderDto
            {
                Id = p.ProviderId,
                Name = p.BusinessName,
                Category = activeServices
                    .OrderBy(s => s.ProviderServiceId)
                    .Select(s => s.Category?.Name ?? "")
                    .FirstOrDefault() ?? "",
                Area = p.ServiceArea,
                Experience = p.Experience,
                Rating = p.Reviews.Any()
                    ? Math.Round(p.Reviews.Average(r => r.Rating), 1)
                    : 0,
                Price = activeServices.Any()
                    ? activeServices.Min(s => s.BasePrice)
                    : 0,
                Image = string.IsNullOrWhiteSpace(p.Image) ? DefaultImage : p.Image,
                Description = p.Description ?? "",
                Available = p.Available,
                Verified = p.IsVerified,
                Services = activeServices
                    .OrderBy(s => s.ProviderServiceId)
                    .Select(s => new ProviderServiceSummaryDto
                    {
                        Id = s.ProviderServiceId,
                        CategoryId = s.CategoryId,
                        Category = s.Category?.Name ?? "",
                        ServiceName = s.ServiceName,
                        Description = s.Description ?? "",
                        ImageUrl = s.ImageUrl,
                        BasePrice = s.BasePrice
                    })
                    .ToList()
            };
        }
    }
}
