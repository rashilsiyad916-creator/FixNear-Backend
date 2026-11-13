using System.Security.Claims;
using FixNear.Application.DTOs.Category;
using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;
using FixNear.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixNear.Controllers
{
    /// <summary>
    /// Authenticated endpoints used by the Provider Studio (business profile,
    /// services, bookings, analytics, categories, and reviews).
    /// </summary>
    [ApiController]
    [Route("api/provider")]
    [Authorize(Roles = "Provider")]
    [Tags("Provider Studio")]
    public class ProviderPortalController : ControllerBase
    {
        private readonly IProviderPortalService _service;

        public ProviderPortalController(IProviderPortalService service)
        {
            _service = service;
        }

        /// <summary>Retrieves authenticated provider profile, catalog services, and settings.</summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProviderProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProviderProfileDto>> GetProfile()
        {
            var profile = await _service.GetProfileAsync(CurrentUserId());
            if (profile == null)
                return NotFound(new { message = "Provider profile not found" });

            return Ok(profile);
        }

        /// <summary>Creates or updates provider business information, pricing, and coverage area.</summary>
        [HttpPost("me")]
        [ProducesResponseType(typeof(ProviderProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProviderProfileDto>> SaveProfile([FromBody] ProviderProfileRequest request)
        {
            return Ok(await _service.SaveProfileAsync(CurrentUserId(), request));
        }

        /// <summary>Uploads provider avatar/cover image directly to Cloudinary.</summary>
        [HttpPost("me/photo")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ImageUploadResponse>> UploadProfilePhoto(IFormFile? file)
        {
            if (!IsSupportedImage(file, out var error))
                return BadRequest(new { message = error });

            return Ok(await _service.UploadPhotoAsync(
                CurrentUserId(), file!.OpenReadStream(), file.FileName));
        }

        /// <summary>Uploads a service showcase image to Cloudinary for service cards.</summary>
        [HttpPost("services/photo")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ImageUploadResponse>> UploadServicePhoto(IFormFile? file)
        {
            if (!IsSupportedImage(file, out var error))
                return BadRequest(new { message = error });

            return Ok(await _service.UploadServicePhotoAsync(
                CurrentUserId(), file!.OpenReadStream(), file.FileName));
        }

        /// <summary>Lists all services currently offered by the provider.</summary>
        [HttpGet("services")]
        [ProducesResponseType(typeof(List<ProviderServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProviderServiceDto>>> ListServices()
        {
            return Ok(await _service.GetServicesAsync(CurrentUserId()));
        }

        /// <summary>Adds a new service offering to the provider's catalog.</summary>
        [HttpPost("services")]
        [ProducesResponseType(typeof(ProviderServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProviderServiceDto>> CreateService([FromBody] ProviderServiceRequest request)
        {
            return Ok(await _service.AddServiceAsync(CurrentUserId(), request));
        }

        /// <summary>Updates an existing service offering's details, pricing, or showcase image.</summary>
        [HttpPut("services/{id:int}")]
        [ProducesResponseType(typeof(ProviderServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProviderServiceDto>> UpdateService(int id, [FromBody] ProviderServiceRequest request)
        {
            var service = await _service.UpdateServiceAsync(CurrentUserId(), id, request);
            if (service == null) return NotFound(new { message = "Service not found" });
            return Ok(service);
        }

        /// <summary>Deletes a service offering from the provider catalog.</summary>
        [HttpDelete("services/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteService(int id)
        {
            if (!await _service.DeleteServiceAsync(CurrentUserId(), id))
                return NotFound(new { message = "Service not found" });

            return Ok(new { deleted = true });
        }

        /// <summary>Lists all bookings received by this provider.</summary>
        [HttpGet("bookings")]
        [ProducesResponseType(typeof(List<ProviderBookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProviderBookingDto>>> ListBookings()
        {
            return Ok(await _service.GetBookingsAsync(CurrentUserId()));
        }

        /// <summary>Updates booking progress status (Confirmed, InProgress, Completed, Cancelled).</summary>
        [HttpPut("bookings/{id:int}/status")]
        [ProducesResponseType(typeof(ProviderBookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProviderBookingDto>> UpdateBookingStatus(
            int id,
            [FromBody] UpdateBookingStatusRequest request)
        {
            var booking = await _service.UpdateBookingStatusAsync(CurrentUserId(), id, request.Status);
            if (booking == null) return NotFound(new { message = "Booking not found" });
            return Ok(booking);
        }

        /// <summary>Toggles instant online availability for customer bookings.</summary>
        [HttpPut("availability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateAvailability([FromBody] UpdateAvailabilityRequest request)
        {
            var available = await _service.SetAvailabilityAsync(CurrentUserId(), request.Available);
            return Ok(new { available });
        }

        /// <summary>Retrieves provider performance statistics, rating, bookings count, and revenue.</summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ProviderStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProviderStatsDto>> GetDashboardStats()
        {
            var stats = await _service.GetStatsAsync(CurrentUserId());
            if (stats == null)
                return NotFound(new { message = "Provider profile not found" });

            return Ok(stats);
        }

        /// <summary>Lists customer reviews received by this provider.</summary>
        [HttpGet("reviews")]
        [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReviewDto>>> ListReviews()
        {
            return Ok(await _service.GetReviewsAsync(CurrentUserId()));
        }

        /// <summary>Lists category addition requests submitted by this provider.</summary>
        [HttpGet("category-requests")]
        [ProducesResponseType(typeof(List<CategoryRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CategoryRequestDto>>> ListCategoryRequests()
        {
            return Ok(await _service.GetCategoryRequestsAsync(CurrentUserId()));
        }

        /// <summary>Proposes a brand-new service category for administrator approval.</summary>
        [HttpPost("category-requests")]
        [ProducesResponseType(typeof(CategoryRequestDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CategoryRequestDto>> SubmitCategoryRequest(
            [FromBody] CreateCategoryRequest request)
        {
            var created = await _service.SubmitCategoryRequestAsync(CurrentUserId(), request);
            return CreatedAtAction(nameof(ListCategoryRequests), new { }, created);
        }

        private static bool IsSupportedImage(IFormFile? file, out string error)
        {
            if (file == null || file.Length == 0)
            {
                error = "No file uploaded";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            {
                error = "Only JPG, PNG or WEBP images are allowed";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")!);
    }
}
