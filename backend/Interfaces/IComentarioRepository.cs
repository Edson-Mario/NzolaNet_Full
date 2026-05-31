using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface IComentarioRepository
    {
        Task<IEnumerable<Comentario>> GetByPublicacaoAsync(int publicacaoId);
        Task<Comentario> CreateAsync(Comentario comentario);
        Task UpdateAsync(Comentario comentario);
        Task DeleteAsync(int id);
        Task<Comentario?> GetByIdAsync(int id);
    }
}
