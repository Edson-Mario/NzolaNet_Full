using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Data;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Repositories
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly NzolaNetDbContext _context;

        public NotificacaoRepository(NzolaNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notificacao>> GetByUtilizadorAsync(int utilizadorId)
        {
            return await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notificacao> CreateAsync(Notificacao notificacao)
        {
            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
            return notificacao;
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif != null)
            {
                notif.Lida = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int utilizadorId)
        {
            await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId && !n.Lida)
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.Lida, true));
        }

        public async Task<int> CountUnreadAsync(int utilizadorId)
        {
            return await _context.Notificacoes
                .CountAsync(n => n.UtilizadorId == utilizadorId && !n.Lida);
        }

        public async Task DeleteAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif != null)
            {
                _context.Notificacoes.Remove(notif);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllByUtilizadorAsync(int utilizadorId)
        {
            await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId)
                .ExecuteDeleteAsync();
        }
    }
}
