using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.DTOs;
using NzolaNet.API.Models;

namespace NzolaNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly NzolaNetDbContext _context;

        public AdminController(NzolaNetDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboard()
        {
            return Ok(new DashboardStatsDto
            {
                TotalUtilizadores = await _context.Utilizadores.CountAsync(),
                TotalPublicacoes = await _context.Publicacoes.CountAsync(),
                TotalComentarios = await _context.Comentarios.CountAsync(),
                TotalBazes = await _context.Bazes.CountAsync(),
                UtilizadoresAtivos = await _context.Utilizadores.CountAsync(u => u.IsActive),
                UtilizadoresDesativados = await _context.Utilizadores.CountAsync(u => !u.IsActive)
            });
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetUsers(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Utilizadores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Nome.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.Endereco != null && u.Endereco.Contains(search)));
            }

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    DataNascimento = u.DataNascimento,
                    Endereco = u.Endereco,
                    Nacionalidade = u.Nacionalidade,
                    Sexo = u.Sexo,
                    CreatedAt = u.CreatedAt,
                    PublicacoesCount = u.Publicacoes.Count,
                    BazesCount = u.Bazes.Count,
                    SeguidoresCount = _context.Seguidores.Count(s => s.SeguidoId == u.Id),
                    SeguindoCount = _context.Seguidores.Count(s => s.SeguidorId == u.Id)
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("users/active")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetActiveUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var users = await _context.Utilizadores
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    DataNascimento = u.DataNascimento,
                    Endereco = u.Endereco,
                    Nacionalidade = u.Nacionalidade,
                    Sexo = u.Sexo,
                    CreatedAt = u.CreatedAt,
                    PublicacoesCount = u.Publicacoes.Count,
                    BazesCount = u.Bazes.Count,
                    SeguidoresCount = _context.Seguidores.Count(s => s.SeguidoId == u.Id),
                    SeguindoCount = _context.Seguidores.Count(s => s.SeguidorId == u.Id)
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("users/inactive")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetInactiveUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var users = await _context.Utilizadores
                .Where(u => !u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    DataNascimento = u.DataNascimento,
                    Endereco = u.Endereco,
                    Nacionalidade = u.Nacionalidade,
                    Sexo = u.Sexo,
                    CreatedAt = u.CreatedAt,
                    PublicacoesCount = u.Publicacoes.Count,
                    BazesCount = u.Bazes.Count,
                    SeguidoresCount = _context.Seguidores.Count(s => s.SeguidoId == u.Id),
                    SeguindoCount = _context.Seguidores.Count(s => s.SeguidorId == u.Id)
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("top-bazes")]
        public async Task<ActionResult<IEnumerable<TopBazesUserDto>>> GetTopBazes()
        {
            var users = await _context.Utilizadores
                .OrderByDescending(u => u.Bazes.Count)
                .Take(10)
                .Select(u => new TopBazesUserDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    FotoPerfil = u.FotoPerfil,
                    BazesCount = u.Bazes.Count,
                    PublicacoesCount = u.Publicacoes.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<PostListItemDto>>> GetPosts(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Publicacoes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var now = DateTime.UtcNow;
                query = filter switch
                {
                    "hora" => query.Where(p => p.CreatedAt >= now.AddHours(-1)),
                    "dia" => query.Where(p => p.CreatedAt >= now.AddDays(-1)),
                    "semana" => query.Where(p => p.CreatedAt >= now.AddDays(-7)),
                    "mes" => query.Where(p => p.CreatedAt >= now.AddMonths(-1)),
                    "ano" => query.Where(p => p.CreatedAt >= now.AddYears(-1)),
                    _ => query
                };
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostListItemDto
                {
                    Id = p.Id,
                    AutorNome = p.Utilizador.Nome,
                    AutorFoto = p.Utilizador.FotoPerfil,
                    Texto = p.Texto,
                    BazesCount = p.Bazes.Count,
                    ComentariosCount = p.Comentarios.Count,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("comments")]
        public async Task<ActionResult<IEnumerable<CommentListItemDto>>> GetComments(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Comentarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var now = DateTime.UtcNow;
                query = filter switch
                {
                    "hora" => query.Where(c => c.CreatedAt >= now.AddHours(-1)),
                    "dia" => query.Where(c => c.CreatedAt >= now.AddDays(-1)),
                    "semana" => query.Where(c => c.CreatedAt >= now.AddDays(-7)),
                    "mes" => query.Where(c => c.CreatedAt >= now.AddMonths(-1)),
                    "ano" => query.Where(c => c.CreatedAt >= now.AddYears(-1)),
                    _ => query
                };
            }

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentListItemDto
                {
                    Id = c.Id,
                    PublicacaoId = c.PublicacaoId,
                    AutorNome = c.Utilizador.Nome,
                    Texto = c.Texto,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string role)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = role;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Role == "admin")
                return BadRequest(new { message = "Nao e possivel desativar um admin." });

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Role == "admin")
                return BadRequest(new { message = "Nao e possivel eliminar um admin." });

            _context.Utilizadores.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("users/inactive")]
        public async Task<IActionResult> RemoveInactiveUsers()
        {
            var removed = await _context.Utilizadores
                .Where(u => !u.IsActive && u.Role != "admin")
                .ExecuteDeleteAsync();

            return Ok(new { removed });
        }

        [HttpDelete("publicacoes/{id}")]
        public async Task<IActionResult> DeletePublicacao(int id)
        {
            var pub = await _context.Publicacoes.FindAsync(id);
            if (pub == null) return NotFound();

            _context.Publicacoes.Remove(pub);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("comentarios/{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comment = await _context.Comentarios.FindAsync(id);
            if (comment == null) return NotFound();

            _context.Comentarios.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            var existing = await _context.Utilizadores
                .AnyAsync(u => u.Email == dto.Email);
            if (existing)
                return BadRequest(new { message = "Este email ja existe no sistema." });

            var user = new Utilizador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Role = "admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Utilizadores.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin criado com sucesso.", id = user.Id });
        }

        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Utilizadores.FindAsync(userId);
            if (user == null) return NotFound();

            var emailExists = await _context.Utilizadores
                .AnyAsync(u => u.Email == dto.NovoEmail && u.Id != userId);
            if (emailExists)
                return BadRequest(new { message = "Este email ja existe no sistema." });

            user.Email = dto.NovoEmail;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Email alterado com sucesso." });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto.NovaSenha != dto.ConfirmarSenha)
                return BadRequest(new { message = "As senhas nao coincidem." });

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Utilizadores.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, user.Senha))
                return BadRequest(new { message = "Senha atual incorreta." });

            user.Senha = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Senha alterada com sucesso." });
        }
    }
}
