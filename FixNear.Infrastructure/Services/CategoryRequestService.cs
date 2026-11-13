using FixNear.Application.DTOs.Category;
using FixNear.Application.Exceptions;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;

namespace FixNear.Infrastructure.Services
{
    public class CategoryRequestService : ICategoryRequestService
    {
        private const int MaxNameLength = 100;
        private const int MaxDescriptionLength = 300;
        private const int MaxNoteLength = 500;

        private readonly ICategoryRequestRepository _repository;

        public CategoryRequestService(ICategoryRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryRequestDto>> GetForProviderAsync(int userId)
        {
            var provider = await _repository.GetProviderByUserIdAsync(userId)
                ?? throw new NotFoundException("Provider profile not found. Complete onboarding first.");

            var requests = await _repository.GetForProviderAsync(provider.ProviderId);
            return requests.Select(Map).ToList();
        }

        public async Task<CategoryRequestDto> SubmitAsync(int userId, CreateCategoryRequest request)
        {
            var provider = await _repository.GetProviderByUserIdAsync(userId)
                ?? throw new NotFoundException("Provider profile not found. Complete onboarding first.");

            var name = NormalizeName(request.Name);
            if (name.Length < 2)
                throw new BadRequestException("Category name must be at least 2 characters");
            if (name.Length > MaxNameLength)
                throw new BadRequestException($"Category name must be {MaxNameLength} characters or fewer");

            var description = (request.Description ?? string.Empty).Trim();
            if (description.Length > MaxDescriptionLength)
                throw new BadRequestException(
                    $"Category description must be {MaxDescriptionLength} characters or fewer");

            var existing = await _repository.FindCategoryByNameAsync(name);
            if (existing is { IsActive: true })
                throw new ConflictException("That category already exists. Pick it from the list.");

            if (await _repository.HasPendingRequestAsync(provider.ProviderId, name))
                throw new ConflictException("You already have a pending request for that category");

            var entity = new CategoryRequest
            {
                ProviderId = provider.ProviderId,
                Name = name,
                Description = description.Length == 0 ? null : description,
                Status = CategoryRequest.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddRequestAsync(entity);
            await _repository.SaveChangesAsync();

            entity.Provider = provider;
            return Map(entity);
        }

        public async Task<List<CategoryRequestDto>> GetAllAsync(string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) && !IsKnownStatus(status.Trim()))
                throw new BadRequestException("Status must be Pending, Approved or Rejected");

            var requests = await _repository.GetByStatusAsync(status?.Trim());
            return requests.Select(Map).ToList();
        }

        public async Task<CategoryRequestDto?> ApproveAsync(int adminUserId, int requestId, string? note)
        {
            var request = await _repository.GetByIdAsync(requestId);
            if (request == null) return null;
            if (request.Status != CategoryRequest.Pending)
                throw new BadRequestException($"This request was already {request.Status.ToLowerInvariant()}");

            var category = await _repository.FindCategoryByNameAsync(request.Name);
            if (category == null)
            {
                category = new Category
                {
                    Name = request.Name,
                    Description = NormalizeNote(request.Description, MaxDescriptionLength),
                    IsActive = true
                };
                await _repository.AddCategoryAsync(category);
            }
            else
            {
                // Re-activate a category that was retired earlier, and fill in a
                // description only when the existing one is empty.
                category.IsActive = true;
                if (string.IsNullOrWhiteSpace(category.Description) &&
                    !string.IsNullOrWhiteSpace(request.Description))
                {
                    category.Description = NormalizeNote(request.Description, MaxDescriptionLength);
                }
            }

            request.Status = CategoryRequest.Approved;
            request.AdminNote = NormalizeNote(note, MaxNoteLength);
            request.ReviewedByUserId = adminUserId;
            request.ReviewedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return Map(request);
        }

        public async Task<CategoryRequestDto?> RejectAsync(int adminUserId, int requestId, string? note)
        {
            var request = await _repository.GetByIdAsync(requestId);
            if (request == null) return null;
            if (request.Status != CategoryRequest.Pending)
                throw new BadRequestException($"This request was already {request.Status.ToLowerInvariant()}");

            request.Status = CategoryRequest.Rejected;
            request.AdminNote = NormalizeNote(note, MaxNoteLength);
            request.ReviewedByUserId = adminUserId;
            request.ReviewedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return Map(request);
        }

        private static string NormalizeName(string? name)
        {
            var cleaned = (name ?? string.Empty).Trim();
            // Collapse repeated whitespace so "AC   Repair" and "AC Repair" match.
            return string.Join(' ', cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private static string? NormalizeNote(string? value, int maxLength)
        {
            var cleaned = (value ?? string.Empty).Trim();
            if (cleaned.Length == 0) return null;
            return cleaned.Length > maxLength ? cleaned[..maxLength] : cleaned;
        }

        private static bool IsKnownStatus(string status) =>
            status == CategoryRequest.Pending ||
            status == CategoryRequest.Approved ||
            status == CategoryRequest.Rejected;

        private static CategoryRequestDto Map(CategoryRequest request) => new()
        {
            Id = request.CategoryRequestId,
            ProviderId = request.ProviderId,
            ProviderName = request.Provider?.BusinessName ?? "Provider",
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Status = request.Status,
            AdminNote = request.AdminNote ?? string.Empty,
            CreatedAt = request.CreatedAt.ToString("yyyy-MM-dd"),
            ReviewedAt = request.ReviewedAt?.ToString("yyyy-MM-dd")
        };
    }
}
