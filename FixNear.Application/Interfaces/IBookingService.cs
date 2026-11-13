using FixNear.Application.DTOs.Booking;
using FixNear.Application.DTOs.Provider;

namespace FixNear.Application.Interfaces
{
    public enum CancelStatus { Success, NotFound, NotPending }

    public enum AddReviewStatus { Success, NotFound, NotCompleted, AlreadyReviewed }

    public class CancelBookingResult
    {
        public CancelStatus Status { get; init; }
        public BookingDto? Booking { get; init; }
    }

    public class AddReviewResult
    {
        public AddReviewStatus Status { get; init; }
        public ReviewDto? Review { get; init; }
    }

    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllForUserAsync(int userId);
        Task<BookingDto?> CreateBookingAsync(int userId, BookingRequest request);
        Task<CancelBookingResult> CancelBookingAsync(int userId, int bookingId);
        Task<AddReviewResult> AddReviewAsync(int userId, int bookingId, ReviewRequest request);
    }
}
