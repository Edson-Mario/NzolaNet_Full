using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface ISeguidorRepository
    {
        Task<Seguidor?> GetAsync(int seguidorId, int seguidoId);
        Task<Seguidor> CreateAsync(Seguidor seguidor);
        Task DeleteAsync(int seguidorId, int seguidoId);
        Task<int> CountSeguidoresAsync(int utilizadorId);
        Task<int> CountSeguindoAsync(int utilizadorId);
        Task<IEnumerable<int>> GetSeguindoIdsAsync(int utilizadorId);
        Task<IEnumerable<Utilizador>> GetSeguidoresAsync(int utilizadorId);
        Task<IEnumerable<Utilizador>> GetSeguindoAsync(int utilizadorId);
    }
}
