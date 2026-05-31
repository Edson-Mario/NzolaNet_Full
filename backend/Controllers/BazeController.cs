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
    public class BazeController : ControllerBase
    {
        private readonly IBazeRepository _bazeRepo;
        private readonly IPublicacaoRepository _pubRepo;
        private readonly INotificacaoRepository _notifRepo;
        private readonly IHubContext<NzolaNetHub> _hub;

        public BazeController(
            IBazeRepository bazeRepo,
            IPublicacaoRepository pubRepo,
            INotificacaoRepository notifRepo,
            IHubContext<NzolaNetHub> hub)
        {
            _bazeRepo = bazeRepo;
            _pubRepo = pubRepo;
            _notifRepo = notifRepo;
            _hub = hub;
        }

        [HttpPost("{publicacaoId}")]
        public async Task<IActionResult> ToggleBaze(int publicacaoId)
        {
            var userId = GetCurrentUserId();
            var existing = await _bazeRepo.GetByPublicacaoAndUserAsync(publicacaoId, userId);

            if (existing != null)
            {
                await _bazeRepo.DeleteAsync(existing.Id);
                await _hub.Clients.All.SendAsync("BazeToggled", publicacaoId, false, 0);
                return Ok(new { bazed = false });
            }

            await _bazeRepo.CreateAsync(new Baze
            {
                PublicacaoId = publicacaoId,
                UtilizadorId = userId
            });

            var pub = await _pubRepo.GetByIdAsync(publicacaoId);
            if (pub != null && pub.UtilizadorId != userId)
            {
                var notif = await _notifRepo.CreateAsync(new Notificacao
                {
                    UtilizadorId = pub.UtilizadorId,
                    Tipo = "baze",
                    Mensagem = $"gostou da sua publicação"
                });

                var unreadCount = await _notifRepo.CountUnreadAsync(pub.UtilizadorId);
                await _hub.Clients.All.SendAsync("NewNotification", new NotificationDto
                {
                    Id = notif.Id,
                    UtilizadorId = notif.UtilizadorId,
                    Tipo = notif.Tipo,
                    Mensagem = notif.Mensagem,
                    Lida = notif.Lida,
                    CreatedAt = notif.CreatedAt
                }, unreadCount);
            }

            await _hub.Clients.All.SendAsync("BazeToggled", publicacaoId, true, 0);

            return Ok(new { bazed = true });
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
