using System.Security.Claims;
using FixNear.Application.Interfaces;
using FixNear.Application.DTOs.Booking;
using FixNear.Application.DTOs.Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixNear.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("Customer Bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        /// <summary>Lists all bookings requested by the authenticated customer.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> ListMyBookings()
        {
            return Ok(await _service.GetAllForUserAsync(CurrentUserId()));
        }

        /// <summary>Creates a new service booking with a provider.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingRequest request)
        {
            if (!DateOnly.TryParseExact(request.Date, "yyyy-MM-dd", out var date))
                return BadRequest(new { message = "Date must be in YYYY-MM-DD format" });
            if (date < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest(new { message = "Booking date cannot be in the past" });
            if (string.IsNullOrWhiteSpace(request.Address))
                return BadRequest(new { message = "Address is required" });

            var booking = await _service.CreateBookingAsync(CurrentUserId(), request);
            if (booking == null)
                return NotFound(new { message = "Provider not found or has no active services" });

            return Ok(booking);
        }

        /// <summary>Cancels a customer's pending service booking.</summary>
        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> CancelBooking(int id)
        {
            var result = await _service.CancelBookingAsync(CurrentUserId(), id);

            return result.Status switch
            {
                CancelStatus.Success => Ok(result.Booking),
                CancelStatus.NotPending => BadRequest(new { message = "Only pending bookings can be cancelled" }),
                _ => NotFound(new { message = "Booking not found" })
            };
        }

        /// <summary>Submits a star rating and written review for a completed booking.</summary>
        [HttpPost("{id:int}/review")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReviewDto>> SubmitBookingReview(int id, [FromBody] ReviewRequest request)
        {
            if (request.Rating is < 1 or > 5)
                return BadRequest(new { message = "Rating must be between 1 and 5" });

            if (!string.IsNullOrWhiteSpace(request.Comment) && request.Comment.Trim().Length > 1000)
                return BadRequest(new { message = "Review comment must be 1000 characters or fewer" });

            var result = await _service.AddReviewAsync(CurrentUserId(), id, request);

            return result.Status switch
            {
                AddReviewStatus.Success => Ok(result.Review),
                AddReviewStatus.NotCompleted => BadRequest(new { message = "You can review this service once it is completed" }),
                AddReviewStatus.AlreadyReviewed => Conflict(new { message = "This booking has already been reviewed" }),
                _ => NotFound(new { message = "Booking not found" })
            };
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")!);
    }
}
