using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly NzolaNetDbContext _context;

        public ComentarioRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comentario>> GetByPublicacaoAsync(int publicacaoId)
        {
            return await _context.Comentarios
                .Include(c => c.Utilizador)
                .Where(c => c.PublicacaoId == publicacaoId)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Comentario?> GetByIdAsync(int id)
        {
            return await _context.Comentarios
                .Include(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comentario> CreateAsync(Comentario comentario)
        {
            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();
            return comentario;
        }

        public async Task UpdateAsync(Comentario comentario)
        {
            _context.Comentarios.Update(comentario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comentarios.FindAsync(id);
            if (comment != null)
            {
                _context.Comentarios.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
