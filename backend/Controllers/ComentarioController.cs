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
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioRepository _commentRepo;
        private readonly INotificacaoRepository _notifRepo;
        private readonly IPublicacaoRepository _pubRepo;
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly IUtilizadorRepository _userRepo;
        private readonly IHubContext<NzolaNetHub> _hub;

        public ComentarioController(
            IComentarioRepository commentRepo,
            INotificacaoRepository notifRepo,
            IPublicacaoRepository pubRepo,
            ISeguidorRepository seguidorRepo,
            IUtilizadorRepository userRepo,
            IHubContext<NzolaNetHub> hub)
        {
            _commentRepo = commentRepo;
            _notifRepo = notifRepo;
            _pubRepo = pubRepo;
            _seguidorRepo = seguidorRepo;
            _userRepo = userRepo;
            _hub = hub;
        }

        [HttpGet("publicacao/{publicacaoId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByPublicacao(int publicacaoId)
        {
            var comments = await _commentRepo.GetByPublicacaoAsync(publicacaoId);

            return Ok(comments.Select(c => new CommentDto
            {
                Id = c.Id,
                PublicacaoId = c.PublicacaoId,
                UtilizadorId = c.UtilizadorId,
                AutorNome = c.Utilizador.Nome,
                AutorFoto = c.Utilizador.FotoPerfil,
                Texto = c.Texto,
                CreatedAt = c.CreatedAt
            }));
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create(CreateCommentDto dto)
        {
            var userId = GetCurrentUserId();

            var pub = await _pubRepo.GetByIdAsync(dto.PublicacaoId);
            if (pub == null) return NotFound(new { message = "Publicação não encontrada." });

            if (pub.UtilizadorId != userId)
            {
                var isFollowing = await _seguidorRepo.GetAsync(userId, pub.UtilizadorId);
                if (isFollowing == null)
                    return BadRequest(new { message = "Só pode comentar em publicações de utilizadores que segue." });
            }

            var comentario = new Comentario
            {
                PublicacaoId = dto.PublicacaoId,
                UtilizadorId = userId,
                Texto = dto.Texto
            };

            await _commentRepo.CreateAsync(comentario);
            var autor = await _userRepo.GetByIdAsync(userId);

            if (pub.UtilizadorId != userId)
            {
                var notif = await _notifRepo.CreateAsync(new Notificacao
                {
                    UtilizadorId = pub.UtilizadorId,
                    Tipo = "comentario",
                    Mensagem = $"comentou na sua publicação"
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

            var commentDto = new CommentDto
            {
                Id = comentario.Id,
                PublicacaoId = comentario.PublicacaoId,
                UtilizadorId = comentario.UtilizadorId,
                AutorNome = autor?.Nome ?? "",
                AutorFoto = autor?.FotoPerfil,
                Texto = comentario.Texto,
                CreatedAt = comentario.CreatedAt
            };

            await _hub.Clients.All.SendAsync("NewComment", commentDto);

            return Ok(commentDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> Update(int id, UpdateCommentDto dto)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null) return NotFound();

            var userId = GetCurrentUserId();
            if (comment.UtilizadorId != userId)
                return Forbid();

            comment.Texto = dto.Texto;
            await _commentRepo.UpdateAsync(comment);

            return Ok(new CommentDto
            {
                Id = comment.Id,
                PublicacaoId = comment.PublicacaoId,
                UtilizadorId = comment.UtilizadorId,
                AutorNome = comment.Utilizador.Nome,
                AutorFoto = comment.Utilizador.FotoPerfil,
                Texto = comment.Texto,
                CreatedAt = comment.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null) return NotFound();

            var userId = GetCurrentUserId();
            if (comment.UtilizadorId != userId)
                return Forbid();

            await _commentRepo.DeleteAsync(id);
            return Ok();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
