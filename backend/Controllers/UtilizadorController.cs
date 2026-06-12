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
    public class UtilizadorController : ControllerBase
    {
        private readonly IUtilizadorRepository _userRepo;
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly IPublicacaoRepository _pubRepo;
        private readonly INotificacaoRepository _notifRepo;
        private readonly IHubContext<NzolaNetHub> _hub;

        public UtilizadorController(
            IUtilizadorRepository userRepo,
            ISeguidorRepository seguidorRepo,
            IPublicacaoRepository pubRepo,
            INotificacaoRepository notifRepo,
            IHubContext<NzolaNetHub> hub)
        {
            _userRepo = userRepo;
            _seguidorRepo = seguidorRepo;
            _pubRepo = pubRepo;
            _notifRepo = notifRepo;
            _hub = hub;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = GetCurrentUserId();
            var isFollowing = currentUserId != id
                ? await _seguidorRepo.GetAsync(currentUserId, id) != null
                : false;

            return Ok(new UserDto
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                FotoPerfil = user.FotoPerfil,
                Privacidade = user.Privacidade,
                Role = user.Role,
                IsActive = user.IsActive,
                PublicacoesCount = await _pubRepo.CountByUtilizadorAsync(id),
                SeguidoresCount = await _seguidorRepo.CountSeguidoresAsync(id),
                SeguindoCount = await _seguidorRepo.CountSeguindoAsync(id),
                IsFollowing = isFollowing
            });
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return NotFound();

            if (dto.Nome != null) user.Nome = dto.Nome;
            if (dto.FotoPerfil != null) user.FotoPerfil = dto.FotoPerfil;
            if (dto.Privacidade != null) user.Privacidade = dto.Privacidade;

            await _userRepo.UpdateAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                FotoPerfil = user.FotoPerfil,
                Privacidade = user.Privacidade,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> Follow(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == id) return BadRequest(new { message = "Não pode seguir a si mesmo." });

            var existing = await _seguidorRepo.GetAsync(userId, id);
            if (existing != null)
                return BadRequest(new { message = "Já está a seguir este utilizador." });

            await _seguidorRepo.CreateAsync(new Seguidor
            {
                SeguidorId = userId,
                SeguidoId = id
            });

            var currentUser = await _userRepo.GetByIdAsync(userId);
            var notif = await _notifRepo.CreateAsync(new Notificacao
            {
                UtilizadorId = id,
                Tipo = "seguidor",
                Mensagem = $"{currentUser?.Nome} começou a seguir-te"
            });

            var unreadCount = await _notifRepo.CountUnreadAsync(id);
            await _hub.Clients.All.SendAsync("NewNotification", new NotificationDto
            {
                Id = notif.Id,
                UtilizadorId = notif.UtilizadorId,
                Tipo = notif.Tipo,
                Mensagem = notif.Mensagem,
                Lida = notif.Lida,
                CreatedAt = notif.CreatedAt
            }, unreadCount);

            return Ok(new { message = "Agora está a seguir este utilizador." });
        }

        [HttpDelete("{id}/unfollow")]
        public async Task<IActionResult> Unfollow(int id)
        {
            var userId = GetCurrentUserId();

            var currentUser = await _userRepo.GetByIdAsync(userId);
            await _seguidorRepo.DeleteAsync(userId, id);

            var notif = await _notifRepo.CreateAsync(new Notificacao
            {
                UtilizadorId = id,
                Tipo = "seguidor",
                Mensagem = $"{currentUser?.Nome} deixou de seguir-te"
            });

            var unreadCount = await _notifRepo.CountUnreadAsync(id);
            await _hub.Clients.All.SendAsync("NewNotification", new NotificationDto
            {
                Id = notif.Id,
                UtilizadorId = notif.UtilizadorId,
                Tipo = notif.Tipo,
                Mensagem = notif.Mensagem,
                Lida = notif.Lida,
                CreatedAt = notif.CreatedAt
            }, unreadCount);

            return Ok(new { message = "Deixou de seguir este utilizador." });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            return await GetById(GetCurrentUserId());
        }

        [HttpPut("deactivate")]
        public async Task<IActionResult> Deactivate()
        {
            var userId = GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = false;
            await _userRepo.UpdateAsync(user);
            return Ok(new { message = "Conta desativada com sucesso." });
        }

        [HttpPut("activate")]
        public async Task<IActionResult> Activate()
        {
            var userId = GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = true;
            await _userRepo.UpdateAsync(user);
            return Ok(new { message = "Conta ativada com sucesso." });
        }

        [HttpGet("{id}/seguidores")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSeguidores(int id)
        {
            var seguidores = await _seguidorRepo.GetSeguidoresAsync(id);
            var currentUserId = GetCurrentUserId();
            var seguindoIds = await _seguidorRepo.GetSeguindoIdsAsync(currentUserId);

            var result = seguidores.Select(s => new UserDto
            {
                Id = s.Id,
                Nome = s.Nome,
                FotoPerfil = s.FotoPerfil,
                IsFollowing = seguindoIds.Contains(s.Id)
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}/seguindo")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSeguindo(int id)
        {
            var seguindo = await _seguidorRepo.GetSeguindoAsync(id);
            var currentUserId = GetCurrentUserId();
            var seguindoIds = await _seguidorRepo.GetSeguindoIdsAsync(currentUserId);

            var result = seguindo.Select(s => new UserDto
            {
                Id = s.Id,
                Nome = s.Nome,
                FotoPerfil = s.FotoPerfil,
                IsFollowing = seguindoIds.Contains(s.Id)
            }).ToList();

            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
