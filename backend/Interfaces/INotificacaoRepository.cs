using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface INotificacaoRepository
    {
        Task<IEnumerable<Notificacao>> GetByUtilizadorAsync(int utilizadorId);
        Task<Notificacao> CreateAsync(Notificacao notificacao);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync(int utilizadorId);
        Task<int> CountUnreadAsync(int utilizadorId);
        Task DeleteAsync(int id);
        Task DeleteAllByUtilizadorAsync(int utilizadorId);
    }
}
