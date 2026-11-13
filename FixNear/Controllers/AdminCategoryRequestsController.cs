using System.Security.Claims;
using FixNear.Application.DTOs.Category;
using FixNear.Application.Interfaces;
using FixNear.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixNear.Controllers
{
    /// <summary>Admin-only review queue for provider-proposed categories.</summary>
    [ApiController]
    [Route("api/admin/category-requests")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin Moderation")]
    public class AdminCategoryRequestsController : ControllerBase
    {
        private readonly ICategoryRequestService _service;

        public AdminCategoryRequestsController(ICategoryRequestService service)
        {
            _service = service;
        }

        /// <summary>Lists category proposals from providers, optionally filtered by status (Pending, Approved, Rejected).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<CategoryRequestDto>>> ListCategoryRequests(
            [FromQuery] string? status = CategoryRequest.Pending)
        {
            return Ok(await _service.GetAllAsync(status));
        }

        /// <summary>Approves a provider category request, adding it to the platform's active categories.</summary>
        [HttpPut("{id:int}/approve")]
        [ProducesResponseType(typeof(CategoryRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryRequestDto>> ApproveCategoryRequest(
            int id,
            [FromBody] ReviewCategoryRequest request)
        {
            var approved = await _service.ApproveAsync(CurrentUserId(), id, request.Note);
            if (approved == null) return NotFound(new { message = "Category request not found" });
            return Ok(approved);
        }

        /// <summary>Rejects a provider category request with an optional explanatory note.</summary>
        [HttpPut("{id:int}/reject")]
        [ProducesResponseType(typeof(CategoryRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryRequestDto>> RejectCategoryRequest(
            int id,
            [FromBody] ReviewCategoryRequest request)
        {
            var rejected = await _service.RejectAsync(CurrentUserId(), id, request.Note);
            if (rejected == null) return NotFound(new { message = "Category request not found" });
            return Ok(rejected);
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")!);
    }
}
