using FixNear.Application.DTOs.Booking;
using FixNear.Application.DTOs.Provider;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;

namespace FixNear.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookingDto>> GetAllForUserAsync(int userId)
        {
            // The scheduled day is a calendar date (stored at midnight), so any
            // booking before today is finished even if nothing marked it yet.
            await _repository.CompletePastBookingsAsync(userId, DateTime.Today);

            var bookings = await _repository.GetByCustomerAsync(userId);
            return bookings.Select(Map);
        }

        public async Task<BookingDto?> CreateBookingAsync(int userId, BookingRequest request)
        {
            if (!DateOnly.TryParseExact(request.Date, "yyyy-MM-dd", out var date))
                return null;

            var service = await _repository.GetActiveServiceByProviderAsync(
                request.ProviderId, request.ProviderServiceId);
            if (service == null)
                return null;

            var booking = new Booking
            {
                CustomerId = userId,
                ProviderServiceId = service.ProviderServiceId,
                ScheduledAt = date.ToDateTime(TimeOnly.MinValue),
                Address = request.Address.Trim(),
                Latitude = request.Latitude is >= -90 and <= 90 ? request.Latitude : null,
                Longitude = request.Longitude is >= -180 and <= 180 ? request.Longitude : null,
                ProblemDescription = request.Problem?.Trim() ?? "",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(booking);
            await _repository.SaveChangesAsync();

            return new BookingDto
            {
                Id = booking.BookingId,
                ProviderId = service.ProviderId,
                ProviderName = service.Provider.BusinessName,
                Service = service.ServiceName,
                Date = booking.ScheduledAt.ToString("yyyy-MM-dd"),
                Address = booking.Address,
                Latitude = booking.Latitude,
                Longitude = booking.Longitude,
                Status = booking.Status,
                HasReview = booking.Reviews.Any()
            };
        }

        public async Task<AddReviewResult> AddReviewAsync(int userId, int bookingId, ReviewRequest request)
        {
            var booking = await _repository.GetByIdForCustomerAsync(bookingId, userId);

            if (booking == null)
                return new AddReviewResult { Status = AddReviewStatus.NotFound };

            if (!string.Equals(booking.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                return new AddReviewResult { Status = AddReviewStatus.NotCompleted };

            if (booking.Reviews.Any())
                return new AddReviewResult { Status = AddReviewStatus.AlreadyReviewed };

            var review = new Review
            {
                BookingId = booking.BookingId,
                CustomerId = userId,
                ProviderId = booking.ProviderService.ProviderId,
                Rating = request.Rating,
                Comment = string.IsNullOrWhiteSpace(request.Comment)
                    ? null
                    : request.Comment.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddReviewAsync(review);
            await _repository.SaveChangesAsync();

            return new AddReviewResult
            {
                Status = AddReviewStatus.Success,
                Review = new ReviewDto
                {
                    Id = review.ReviewId,
                    ProviderId = review.ProviderId,
                    Author = booking.Customer.FullName,
                    Rating = review.Rating,
                    Comment = review.Comment ?? string.Empty,
                    Date = review.CreatedAt.ToString("yyyy-MM-dd")
                }
            };
        }

        public async Task<CancelBookingResult> CancelBookingAsync(int userId, int bookingId)
        {
            var booking = await _repository.GetByIdForCustomerAsync(bookingId, userId);

            if (booking == null)
                return new CancelBookingResult { Status = CancelStatus.NotFound };

            if (booking.Status != "Pending")
                return new CancelBookingResult { Status = CancelStatus.NotPending };

            // A pending booking whose day already passed is complete, not cancellable.
            if (booking.ScheduledAt.Date < DateTime.Today)
            {
                booking.Status = "Completed";
                await _repository.SaveChangesAsync();
                return new CancelBookingResult { Status = CancelStatus.NotPending };
            }

            booking.Status = "Cancelled";
            await _repository.SaveChangesAsync();

            return new CancelBookingResult
            {
                Status = CancelStatus.Success,
                Booking = Map(booking)
            };
        }

        private static BookingDto Map(Booking b)
        {
            return new BookingDto
            {
                Id = b.BookingId,
                ProviderId = b.ProviderService.ProviderId,
                ProviderName = b.ProviderService.Provider.BusinessName,
                Service = b.ProviderService.ServiceName,
                Date = b.ScheduledAt.ToString("yyyy-MM-dd"),
                Address = b.Address,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                Status = b.Status,
                HasReview = b.Reviews.Any()
            };
        }
    }
}
