using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NzolaNet.API.DTOs;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;
using NzolaNet.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NzolaNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PublicacaoController : ControllerBase
    {
        private readonly IPublicacaoRepository _pubRepo;
        private readonly IBazeRepository _bazeRepo;
        private readonly IUtilizadorRepository _userRepo;
        private readonly IHubContext<NzolaNetHub> _hub;

        public PublicacaoController(
            IPublicacaoRepository pubRepo,
            IBazeRepository bazeRepo,
            IUtilizadorRepository userRepo,
            IHubContext<NzolaNetHub> hub)
        {
            _pubRepo = pubRepo;
            _bazeRepo = bazeRepo;
            _userRepo = userRepo;
            _hub = hub;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var posts = await _pubRepo.GetFeedDtoAsync(userId, page, pageSize);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetById(int id)
        {
            var pub = await _pubRepo.GetByIdAsync(id);
            if (pub == null) return NotFound();

            var userId = GetCurrentUserId();

            return Ok(new PostDto
            {
                Id = pub.Id,
                UtilizadorId = pub.UtilizadorId,
                AutorNome = pub.Utilizador.Nome,
                AutorFoto = pub.Utilizador.FotoPerfil,
                Texto = pub.Texto,
                Imagem = pub.Imagem,
                Video = pub.Video,
                BazesCount = pub.Bazes.Count,
                ComentariosCount = pub.Comentarios.Count,
                IsBazed = pub.Bazes.Any(b => b.UtilizadorId == userId),
                CreatedAt = pub.CreatedAt,
                UpdatedAt = pub.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> Create(CreatePostDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);

            var pub = new Publicacao
            {
                UtilizadorId = userId,
                Texto = dto.Texto,
                Imagem = dto.Imagem,
                Video = dto.Video
            };

            await _pubRepo.CreateAsync(pub);

            var postDto = new PostDto
            {
                Id = pub.Id,
                UtilizadorId = pub.UtilizadorId,
                AutorNome = user!.Nome,
                AutorFoto = user.FotoPerfil,
                Texto = pub.Texto,
                Imagem = pub.Imagem,
                Video = pub.Video,
                CreatedAt = pub.CreatedAt
            };

            await _hub.Clients.All.SendAsync("NewPost", postDto);

            return CreatedAtAction(nameof(GetById), new { id = pub.Id }, postDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePostDto dto)
        {
            var pub = await _pubRepo.GetByIdAsync(id);
            if (pub == null) return NotFound();

            if (pub.UtilizadorId != GetCurrentUserId())
                return Forbid();

            if (dto.Texto != null) pub.Texto = dto.Texto;
            if (dto.Imagem != null) pub.Imagem = dto.Imagem;
            if (dto.Video != null) pub.Video = dto.Video;

            await _pubRepo.UpdateAsync(pub);

            var userId = GetCurrentUserId();
            var postDto = new PostDto
            {
                Id = pub.Id,
                UtilizadorId = pub.UtilizadorId,
                AutorNome = pub.Utilizador.Nome,
                AutorFoto = pub.Utilizador.FotoPerfil,
                Texto = pub.Texto,
                Imagem = pub.Imagem,
                Video = pub.Video,
                BazesCount = pub.Bazes.Count,
                ComentariosCount = pub.Comentarios.Count,
                IsBazed = pub.Bazes.Any(b => b.UtilizadorId == userId),
                CreatedAt = pub.CreatedAt,
                UpdatedAt = pub.UpdatedAt
            };

            await _hub.Clients.All.SendAsync("PostUpdated", postDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pub = await _pubRepo.GetByIdAsync(id);
            if (pub == null) return NotFound();

            var userId = GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);

            if (pub.UtilizadorId != userId && user?.Role != "admin")
                return Forbid();

            await _pubRepo.DeleteAsync(id);

            await _hub.Clients.All.SendAsync("PostDeleted", id);

            return Ok();
        }

        [HttpGet("user/{utilizadorId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetByUser(
            int utilizadorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var posts = await _pubRepo.GetByUtilizadorDtoAsync(utilizadorId, userId, page, pageSize);
            return Ok(posts);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
