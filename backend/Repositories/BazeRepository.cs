using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class BazeRepository : IBazeRepository
    {
        private readonly NzolaNetDbContext _context;

        public BazeRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<Baze?> GetByPublicacaoAndUserAsync(int publicacaoId, int utilizadorId)
        {
            return await _context.Bazes
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.PublicacaoId == publicacaoId && b.UtilizadorId == utilizadorId);
        }

        public async Task<Baze> CreateAsync(Baze baze)
        {
            _context.Bazes.Add(baze);
            await _context.SaveChangesAsync();
            return baze;
        }

        public async Task DeleteAsync(int id)
        {
            var baze = await _context.Bazes.FindAsync(id);
            if (baze != null)
            {
                _context.Bazes.Remove(baze);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountByPublicacaoAsync(int publicacaoId)
        {
            return await _context.Bazes.CountAsync(b => b.PublicacaoId == publicacaoId);
        }
    }
}
