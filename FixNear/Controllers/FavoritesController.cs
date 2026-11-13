using System.Security.Claims;
using FixNear.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixNear.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("Customer Favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service)
        {
            _service = service;
        }

        /// <summary>Lists all favorite provider IDs for the authenticated customer.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<int>>> ListFavorites()
        {
            return Ok(await _service.GetFavoritesAsync(CurrentUserId()));
        }

        /// <summary>Adds a service provider to customer favorites.</summary>
        [HttpPost("{providerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddFavorite(int providerId)
        {
            await _service.AddFavoriteAsync(CurrentUserId(), providerId);
            return Ok(new { message = "Added to favorites", providerId });
        }

        /// <summary>Removes a service provider from customer favorites.</summary>
        [HttpDelete("{providerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveFavorite(int providerId)
        {
            await _service.RemoveFavoriteAsync(CurrentUserId(), providerId);
            return Ok(new { message = "Removed from favorites", providerId });
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")!);
    }
}
