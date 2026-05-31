using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class UtilizadorRepository : IUtilizadorRepository
    {
        private readonly NzolaNetDbContext _context;

        public UtilizadorRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<Utilizador?> GetByIdAsync(int id)
        {
            return await _context.Utilizadores.FindAsync(id);
        }

        public async Task<Utilizador?> GetByEmailAsync(string email)
        {
            return await _context.Utilizadores
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Utilizador>> GetAllAsync()
        {
            return await _context.Utilizadores
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<Utilizador> CreateAsync(Utilizador utilizador)
        {
            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();
            return utilizador;
        }

        public async Task UpdateAsync(Utilizador utilizador)
        {
            _context.Utilizadores.Update(utilizador);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            if (user != null)
            {
                _context.Utilizadores.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Utilizadores.AnyAsync(u => u.Email == email);
        }
    }
}
