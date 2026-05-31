using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface IBazeRepository
    {
        Task<Baze?> GetByPublicacaoAndUserAsync(int publicacaoId, int utilizadorId);
        Task<Baze> CreateAsync(Baze baze);
        Task DeleteAsync(int id);
        Task<int> CountByPublicacaoAsync(int publicacaoId);
    }
}
