using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.DTOs;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class PublicacaoRepository : IPublicacaoRepository
    {
        private readonly NzolaNetDbContext _context;

        public PublicacaoRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<Publicacao?> GetByIdAsync(int id)
        {
            return await _context.Publicacoes
                .Include(p => p.Utilizador)
                .Include(p => p.Bazes)
                .Include(p => p.Comentarios)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PostDto>> GetFeedDtoAsync(int userId, int page, int pageSize)
        {
            return await _context.Publicacoes
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    UtilizadorId = p.UtilizadorId,
                    AutorNome = p.Utilizador.Nome,
                    AutorFoto = p.Utilizador.FotoPerfil,
                    Texto = p.Texto,
                    Imagem = p.Imagem,
                    Video = p.Video,
                    BazesCount = p.Bazes.Count,
                    ComentariosCount = p.Comentarios.Count,
                    IsBazed = p.Bazes.Any(b => b.UtilizadorId == userId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PostDto>> GetByUtilizadorDtoAsync(int utilizadorId, int userId, int page, int pageSize)
        {
            return await _context.Publicacoes
                .AsNoTracking()
                .Where(p => p.UtilizadorId == utilizadorId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    UtilizadorId = p.UtilizadorId,
                    AutorNome = p.Utilizador.Nome,
                    AutorFoto = p.Utilizador.FotoPerfil,
                    Texto = p.Texto,
                    Imagem = p.Imagem,
                    Video = p.Video,
                    BazesCount = p.Bazes.Count,
                    ComentariosCount = p.Comentarios.Count,
                    IsBazed = p.Bazes.Any(b => b.UtilizadorId == userId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PostDto>> GetFollowingFeedDtoAsync(int utilizadorId, int page, int pageSize)
        {
            var seguindoIds = await _context.Seguidores
                .AsNoTracking()
                .Where(s => s.SeguidorId == utilizadorId)
                .Select(s => s.SeguidoId)
                .ToListAsync();

            return await _context.Publicacoes
                .AsNoTracking()
                .Where(p => seguindoIds.Contains(p.UtilizadorId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    UtilizadorId = p.UtilizadorId,
                    AutorNome = p.Utilizador.Nome,
                    AutorFoto = p.Utilizador.FotoPerfil,
                    Texto = p.Texto,
                    Imagem = p.Imagem,
                    Video = p.Video,
                    BazesCount = p.Bazes.Count,
                    ComentariosCount = p.Comentarios.Count,
                    IsBazed = p.Bazes.Any(b => b.UtilizadorId == utilizadorId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Publicacao>> GetFeedAsync(int page, int pageSize)
        {
            return await _context.Publicacoes
                .Include(p => p.Utilizador)
                .Include(p => p.Bazes)
                .Include(p => p.Comentarios)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Publicacao>> GetByUtilizadorAsync(int utilizadorId, int page, int pageSize)
        {
            return await _context.Publicacoes
                .Include(p => p.Utilizador)
                .Include(p => p.Bazes)
                .Include(p => p.Comentarios)
                .Where(p => p.UtilizadorId == utilizadorId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Publicacao>> GetFollowingFeedAsync(int utilizadorId, int page, int pageSize)
        {
            var seguindoIds = await _context.Seguidores
                .Where(s => s.SeguidorId == utilizadorId)
                .Select(s => s.SeguidoId)
                .ToListAsync();

            return await _context.Publicacoes
                .Include(p => p.Utilizador)
                .Include(p => p.Bazes)
                .Include(p => p.Comentarios)
                .Where(p => seguindoIds.Contains(p.UtilizadorId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Publicacao> CreateAsync(Publicacao publicacao)
        {
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();
            return publicacao;
        }

        public async Task UpdateAsync(Publicacao publicacao)
        {
            publicacao.UpdatedAt = DateTime.UtcNow;
            _context.Publicacoes.Update(publicacao);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pub = await _context.Publicacoes.FindAsync(id);
            if (pub != null)
            {
                _context.Publicacoes.Remove(pub);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountByUtilizadorAsync(int utilizadorId)
        {
            return await _context.Publicacoes.CountAsync(p => p.UtilizadorId == utilizadorId);
        }
    }
}
