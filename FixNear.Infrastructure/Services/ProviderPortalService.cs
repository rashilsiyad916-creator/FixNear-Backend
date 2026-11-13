using FixNear.Application.DTOs.Category;
using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;
using FixNear.Application.Exceptions;
using FixNear.Application.Interfaces;
using FixNear.Application.Models;
using FixNear.Domain.Entities;
using ProviderServiceEntity = FixNear.Domain.Entities.ProviderService;

namespace FixNear.Infrastructure.Services
{
    public class ProviderPortalService : IProviderPortalService
    {
        private const string DefaultImage = "/assets/services/electrician-wiring-pro-v2.jpg";
        private static readonly HashSet<string> AllowedBookingStatuses =
            new(StringComparer.OrdinalIgnoreCase) { "Accepted", "Completed", "Cancelled" };

        private readonly IProviderPortalRepository _repository;
        private readonly IPhotoStorageService _photos;
        private readonly ICategoryRequestService _categoryRequests;

        public ProviderPortalService(
            IProviderPortalRepository repository,
            IPhotoStorageService photos,
            ICategoryRequestService categoryRequests)
        {
            _repository = repository;
            _photos = photos;
            _categoryRequests = categoryRequests;
        }

        public async Task<ProviderProfileDto?> GetProfileAsync(int userId)
        {
            var provider = await _repository.GetByUserIdAsync(userId);
            return provider == null ? null : MapProfile(provider);
        }

        public async Task<ProviderProfileDto> SaveProfileAsync(int userId, ProviderProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.BusinessName))
                throw new BadRequestException("Business name is required");
            if (string.IsNullOrWhiteSpace(request.ServiceArea))
                throw new BadRequestException("Service area is required");
            if (string.IsNullOrWhiteSpace(request.Address))
                throw new BadRequestException("Business address is required");
            if (request.Experience < 0 || request.Experience > 60)
                throw new BadRequestException("Experience must be between 0 and 60 years");

            var provider = await _repository.GetByUserIdAsync(userId);
            var isNew = provider == null;

            if (isNew)
            {
                provider = new Provider
                {
                    UserId = userId,
                    IsVerified = false,
                    Available = request.Available ?? true
                };
                await _repository.AddProviderAsync(provider);
            }

            provider!.BusinessName = request.BusinessName.Trim();
            provider.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            provider.ServiceArea = request.ServiceArea.Trim();
            provider.Address = request.Address.Trim();
            provider.Experience = request.Experience;

            if (request.Available.HasValue)
                provider.Available = request.Available.Value;

            if (!string.IsNullOrWhiteSpace(request.Image))
                provider.Image = request.Image.Trim();
            else if (string.IsNullOrWhiteSpace(provider.Image))
                provider.Image = DefaultImage;

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var user = provider.User ?? await _repository.GetUserAsync(userId);
                if (user != null) user.Phone = request.Phone.Trim();
            }

            await _repository.SaveChangesAsync();

            if (request.CategoryId.HasValue)
                await AddOnboardingServiceAsync(provider, request);

            // Re-read so the DTO reflects the saved service and user navigation.
            var saved = await _repository.GetByUserIdAsync(userId);
            return MapProfile(saved!);
        }

        public async Task<ImageUploadResponse> UploadPhotoAsync(int userId, Stream content, string fileName)
        {
            var provider = await RequireProviderAsync(userId);
            var photo = await _photos.UploadFileAsync(content, fileName);
            provider.Image = photo.Url;
            await _repository.SaveChangesAsync();
            return MapPhoto(photo);
        }

        public async Task<ImageUploadResponse> UploadServicePhotoAsync(int userId, Stream content, string fileName)
        {
            await RequireProviderAsync(userId);
            var photo = await _photos.UploadFileAsync(content, fileName, "fixnear/services");
            return MapPhoto(photo);
        }

        public Task<List<CategoryRequestDto>> GetCategoryRequestsAsync(int userId) =>
            _categoryRequests.GetForProviderAsync(userId);

        public Task<CategoryRequestDto> SubmitCategoryRequestAsync(int userId, CreateCategoryRequest request) =>
            _categoryRequests.SubmitAsync(userId, request);

        public async Task<List<ProviderServiceDto>> GetServicesAsync(int userId)
        {
            var provider = await RequireProviderAsync(userId);
            var services = await _repository.GetServicesAsync(provider.ProviderId);

            var result = new List<ProviderServiceDto>();
            foreach (var service in services)
            {
                var bookings = await _repository.CountBookingsForServiceAsync(service.ProviderServiceId);
                result.Add(MapService(service, bookings));
            }

            return result;
        }

        public async Task<ProviderServiceDto> AddServiceAsync(int userId, ProviderServiceRequest request)
        {
            var provider = await RequireProviderAsync(userId);
            ValidateService(request);

            var category = await _repository.GetCategoryAsync(request.CategoryId)
                ?? throw new BadRequestException("Please choose a valid category");

            var service = new ProviderServiceEntity
            {
                ProviderId = provider.ProviderId,
                CategoryId = category.CategoryId,
                ServiceName = request.ServiceName.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),
                ImageUrl = NormalizeImage(request.ImageUrl),
                BasePrice = request.BasePrice,
                IsActive = request.IsActive,
                Category = category
            };

            await _repository.AddServiceAsync(service);
            await _repository.SaveChangesAsync();

            return MapService(service, 0);
        }

        public async Task<ProviderServiceDto?> UpdateServiceAsync(int userId, int serviceId,
            ProviderServiceRequest request)
        {
            var provider = await RequireProviderAsync(userId);
            var service = await _repository.GetServiceAsync(provider.ProviderId, serviceId);
            if (service == null) return null;

            ValidateService(request);

            var category = await _repository.GetCategoryAsync(request.CategoryId)
                ?? throw new BadRequestException("Please choose a valid category");

            service.CategoryId = category.CategoryId;
            service.Category = category;
            service.ServiceName = request.ServiceName.Trim();
            service.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            service.ImageUrl = NormalizeImage(request.ImageUrl);
            service.BasePrice = request.BasePrice;
            service.IsActive = request.IsActive;

            await _repository.SaveChangesAsync();

            var bookings = await _repository.CountBookingsForServiceAsync(service.ProviderServiceId);
            return MapService(service, bookings);
        }

        public async Task<bool> DeleteServiceAsync(int userId, int serviceId)
        {
            var provider = await RequireProviderAsync(userId);
            var service = await _repository.GetServiceAsync(provider.ProviderId, serviceId);
            if (service == null) return false;

            var bookings = await _repository.CountBookingsForServiceAsync(service.ProviderServiceId);
            if (bookings > 0)
            {
                // Keep history intact: retire the service instead of deleting it.
                service.IsActive = false;
            }
            else
            {
                _repository.RemoveService(service);
            }

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProviderBookingDto>> GetBookingsAsync(int userId)
        {
            var provider = await RequireProviderAsync(userId);
            await _repository.CompletePastBookingsAsync(provider.ProviderId, DateTime.Today);
            var bookings = await _repository.GetBookingsAsync(provider.ProviderId);
            return bookings.Select(MapBooking).ToList();
        }

        public async Task<ProviderBookingDto?> UpdateBookingStatusAsync(int userId, int bookingId, string status)
        {
            if (string.IsNullOrWhiteSpace(status) || !AllowedBookingStatuses.Contains(status.Trim()))
                throw new BadRequestException("Status must be Accepted, Completed or Cancelled");

            var provider = await RequireProviderAsync(userId);
            var booking = await _repository.GetBookingAsync(provider.ProviderId, bookingId);
            if (booking == null) return null;

            var target = AllowedBookingStatuses.First(s =>
                string.Equals(s, status.Trim(), StringComparison.OrdinalIgnoreCase));

            if (booking.Status == target)
                return MapBooking(booking);

            var allowed = target switch
            {
                "Accepted" => booking.Status == "Pending",
                "Completed" => booking.Status is "Pending" or "Accepted",
                "Cancelled" => booking.Status is "Pending" or "Accepted",
                _ => false
            };

            if (!allowed)
                throw new BadRequestException($"Cannot change a {booking.Status} booking to {target}");

            booking.Status = target;
            await _repository.SaveChangesAsync();
            return MapBooking(booking);
        }

        public async Task<bool> SetAvailabilityAsync(int userId, bool available)
        {
            var provider = await RequireProviderAsync(userId);
            provider.Available = available;
            await _repository.SaveChangesAsync();
            return provider.Available;
        }

        public async Task<ProviderStatsDto?> GetStatsAsync(int userId)
        {
            var provider = await _repository.GetByUserIdAsync(userId);
            if (provider == null) return null;

            await _repository.CompletePastBookingsAsync(provider.ProviderId, DateTime.Today);
            var bookings = await _repository.GetBookingsAsync(provider.ProviderId);
            var services = await _repository.GetServicesAsync(provider.ProviderId);

            return new ProviderStatsDto
            {
                TotalBookings = bookings.Count,
                PendingBookings = bookings.Count(b => b.Status == "Pending"),
                AcceptedBookings = bookings.Count(b => b.Status == "Accepted"),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
                TodayBookings = bookings.Count(b =>
                    b.ScheduledAt.Date == DateTime.Today &&
                    b.Status is "Pending" or "Accepted"),
                TotalEarnings = bookings
                    .Where(b => b.Status == "Completed")
                    .Sum(b => b.ProviderService.BasePrice),
                ActiveServices = services.Count(s => s.IsActive),
                TotalServices = services.Count,
                AverageRating = provider.Reviews.Any()
                    ? Math.Round(provider.Reviews.Average(r => r.Rating), 1)
                    : 0,
                ReviewCount = provider.Reviews.Count
            };
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(int userId)
        {
            var provider = await RequireProviderAsync(userId);
            var reviews = await _repository.GetReviewsAsync(provider.ProviderId);

            return reviews.Select(r => new ReviewDto
            {
                Id = r.ReviewId,
                ProviderId = r.ProviderId,
                Author = r.Customer?.FullName ?? "Customer",
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                Date = r.CreatedAt.ToString("yyyy-MM-dd")
            }).ToList();
        }

        private async Task AddOnboardingServiceAsync(Provider provider, ProviderProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ServiceName))
                throw new BadRequestException("Service name is required when a category is selected");

            if (request.BasePrice is < 0)
                throw new BadRequestException("Base price cannot be negative");

            // Only seed a first service when the provider has none yet.
            if (provider.Services.Any()) return;

            var category = await _repository.GetCategoryAsync(request.CategoryId!.Value)
                ?? throw new BadRequestException("Please choose a valid category");

            await _repository.AddServiceAsync(new ProviderServiceEntity
            {
                ProviderId = provider.ProviderId,
                CategoryId = category.CategoryId,
                ServiceName = request.ServiceName.Trim(),
                Description = string.IsNullOrWhiteSpace(request.ServiceDescription)
                    ? null
                    : request.ServiceDescription.Trim(),
                BasePrice = request.BasePrice ?? 0,
                IsActive = true
            });
            await _repository.SaveChangesAsync();
        }

        private async Task<Provider> RequireProviderAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Provider profile not found. Complete onboarding first.");
        }

        private static void ValidateService(ProviderServiceRequest request)
        {
            if (request.CategoryId <= 0)
                throw new BadRequestException("Please choose a category");
            if (string.IsNullOrWhiteSpace(request.ServiceName))
                throw new BadRequestException("Service name is required");
            if (request.BasePrice < 0)
                throw new BadRequestException("Base price cannot be negative");
            if (request.BasePrice > 100000)
                throw new BadRequestException("Base price seems too high");
        }

        private static string? NormalizeImage(string? imageUrl) =>
            string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();

        private static ImageUploadResponse MapPhoto(PhotoUploadResult photo) => new()
        {
            ImageUrl = photo.Url,
            Url = photo.Url,
            SecureUrl = photo.Url,
            PublicId = photo.PublicId,
            Format = photo.Format,
            ResourceType = "image",
            Width = photo.Width,
            Height = photo.Height,
            Bytes = photo.Bytes,
            Folder = photo.Folder,
            Success = true,
            Message = "Image uploaded successfully to Cloudinary",
            CreatedAt = DateTime.UtcNow
        };

        private static ProviderProfileDto MapProfile(Provider provider)
        {
            var services = provider.Services?.ToList() ?? new List<ProviderServiceEntity>();

            return new ProviderProfileDto
            {
                Id = provider.ProviderId,
                UserId = provider.UserId,
                BusinessName = provider.BusinessName,
                Description = provider.Description ?? "",
                ServiceArea = provider.ServiceArea,
                Address = provider.Address,
                Image = string.IsNullOrWhiteSpace(provider.Image) ? DefaultImage : provider.Image,
                Email = provider.User?.Email ?? "",
                Phone = provider.User?.Phone ?? "",
                Experience = provider.Experience,
                Verified = provider.IsVerified,
                Available = provider.Available,
                Rating = provider.Reviews != null && provider.Reviews.Any()
                    ? Math.Round(provider.Reviews.Average(r => r.Rating), 1)
                    : 0,
                ReviewCount = provider.Reviews?.Count ?? 0,
                ActiveServices = services.Count(s => s.IsActive)
            };
        }

        private static ProviderServiceDto MapService(ProviderServiceEntity service, int bookingCount)
        {
            return new ProviderServiceDto
            {
                Id = service.ProviderServiceId,
                CategoryId = service.CategoryId,
                CategoryName = service.Category?.Name ?? "",
                ServiceName = service.ServiceName,
                Description = service.Description ?? "",
                ImageUrl = service.ImageUrl,
                BasePrice = service.BasePrice,
                IsActive = service.IsActive,
                BookingCount = bookingCount
            };
        }

        private static ProviderBookingDto MapBooking(Booking booking)
        {
            return new ProviderBookingDto
            {
                Id = booking.BookingId,
                CustomerName = booking.Customer?.FullName ?? "Customer",
                CustomerEmail = booking.Customer?.Email ?? "",
                CustomerPhone = booking.Customer?.Phone ?? "",
                Service = booking.ProviderService?.ServiceName ?? "",
                Price = booking.ProviderService?.BasePrice ?? 0,
                Date = booking.ScheduledAt.ToString("yyyy-MM-dd"),
                Address = booking.Address,
                Latitude = booking.Latitude,
                Longitude = booking.Longitude,
                Problem = booking.ProblemDescription,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt.ToString("yyyy-MM-dd")
            };
        }
    }
}
