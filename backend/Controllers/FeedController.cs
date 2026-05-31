using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NzolaNet.API.DTOs;
using NzolaNet.API.Interfaces;

namespace NzolaNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedController : ControllerBase
    {
        private readonly IPublicacaoRepository _pubRepo;

        public FeedController(IPublicacaoRepository pubRepo)
        {
            _pubRepo = pubRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetHomeFeed(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var posts = await _pubRepo.GetFeedDtoAsync(userId, page, pageSize);
            return Ok(posts);
        }

        [HttpGet("following")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFollowingFeed(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var posts = await _pubRepo.GetFollowingFeedDtoAsync(userId, page, pageSize);
            return Ok(posts);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
