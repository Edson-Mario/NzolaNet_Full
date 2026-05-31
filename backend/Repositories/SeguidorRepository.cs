using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class SeguidorRepository : ISeguidorRepository
    {
        private readonly NzolaNetDbContext _context;

        public SeguidorRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<Seguidor?> GetAsync(int seguidorId, int seguidoId)
        {
            return await _context.Seguidores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SeguidorId == seguidorId && s.SeguidoId == seguidoId);
        }

        public async Task<Seguidor> CreateAsync(Seguidor seguidor)
        {
            _context.Seguidores.Add(seguidor);
            await _context.SaveChangesAsync();
            return seguidor;
        }

        public async Task DeleteAsync(int seguidorId, int seguidoId)
        {
            var seguidor = await _context.Seguidores
                .FirstOrDefaultAsync(s => s.SeguidorId == seguidorId && s.SeguidoId == seguidoId);
            if (seguidor != null)
            {
                _context.Seguidores.Remove(seguidor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountSeguidoresAsync(int utilizadorId)
        {
            return await _context.Seguidores.CountAsync(s => s.SeguidoId == utilizadorId);
        }

        public async Task<int> CountSeguindoAsync(int utilizadorId)
        {
            return await _context.Seguidores.CountAsync(s => s.SeguidorId == utilizadorId);
        }

        public async Task<IEnumerable<int>> GetSeguindoIdsAsync(int utilizadorId)
        {
            return await _context.Seguidores
                .AsNoTracking()
                .Where(s => s.SeguidorId == utilizadorId)
                .Select(s => s.SeguidoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Utilizador>> GetSeguidoresAsync(int utilizadorId)
        {
            return await _context.Seguidores
                .AsNoTracking()
                .Where(s => s.SeguidoId == utilizadorId)
                .Select(s => s.SeguidorUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Utilizador>> GetSeguindoAsync(int utilizadorId)
        {
            return await _context.Seguidores
                .AsNoTracking()
                .Where(s => s.SeguidorId == utilizadorId)
                .Select(s => s.SeguidoUser)
                .ToListAsync();
        }
    }
}
