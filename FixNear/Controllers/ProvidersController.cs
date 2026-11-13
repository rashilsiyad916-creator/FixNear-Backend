using System.Security.Claims;
using FixNear.Application.DTOs.Media;
using FixNear.Application.DTOs.Provider;
using FixNear.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixNear.Controllers
{
    /// <summary>Public storefront endpoints for browsing service providers and reviews.</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Providers & Storefront")]
    public class ProvidersController : ControllerBase
    {
        private readonly IProviderService _service;
        private readonly IProviderPortalService _portal;

        public ProvidersController(IProviderService service, IProviderPortalService portal)
        {
            _service = service;
            _portal = portal;
        }

        /// <summary>Lists public provider profiles filtered by optional category or service area.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProviderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProviderDto>>> ListProviders(
            [FromQuery] string? category,
            [FromQuery] string? area)
        {
            return Ok(await _service.GetAllAsync(category, area));
        }

        /// <summary>Retrieves a single provider's detailed profile with active services and metrics.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProviderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProviderDto>> GetProvider(int id)
        {
            var provider = await _service.GetByIdAsync(id);
            if (provider == null) return NotFound(new { message = "Provider not found" });
            return Ok(provider);
        }

        /// <summary>Lists customer reviews and ratings for a provider.</summary>
        [HttpGet("{id:int}/reviews")]
        [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListProviderReviews(int id)
        {
            if (!await _service.ExistsAsync(id))
                return NotFound(new { message = "Provider not found" });

            return Ok(await _service.GetReviewsAsync(id));
        }

        /// <summary>
        /// Uploads provider profile photo to Cloudinary.
        /// Primary endpoint is <c>POST /api/provider/me/photo</c>.
        /// </summary>
        [HttpPost("{id:int}/photo")]
        [Authorize(Roles = "Provider")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ImageUploadResponse>> UploadProviderPhoto(int id, IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
                return BadRequest(new { message = "Only JPG, PNG or WEBP images are allowed" });

            var profile = await _portal.GetProfileAsync(CurrentUserId());
            if (profile == null || profile.Id != id) return Forbid();

            return Ok(await _portal.UploadPhotoAsync(
                CurrentUserId(), file.OpenReadStream(), file.FileName));
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")!);
    }
}
