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
    public class NotificacaoController : ControllerBase
    {
        private readonly INotificacaoRepository _notifRepo;

        public NotificacaoController(INotificacaoRepository notifRepo)
        {
            _notifRepo = notifRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            var userId = GetCurrentUserId();
            var notifs = await _notifRepo.GetByUtilizadorAsync(userId);

            return Ok(notifs.Select(n => new NotificationDto
            {
                Id = n.Id,
                UtilizadorId = n.UtilizadorId,
                Tipo = n.Tipo,
                Mensagem = n.Mensagem,
                Lida = n.Lida,
                CreatedAt = n.CreatedAt
            }));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            return Ok(await _notifRepo.CountUnreadAsync(userId));
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notifRepo.MarkAsReadAsync(id);
            return Ok();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            await _notifRepo.MarkAllAsReadAsync(userId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _notifRepo.DeleteAsync(id);
            return Ok();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
