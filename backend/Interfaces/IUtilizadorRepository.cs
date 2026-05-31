using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface IUtilizadorRepository
    {
        Task<Utilizador?> GetByIdAsync(int id);
        Task<Utilizador?> GetByEmailAsync(string email);
        Task<IEnumerable<Utilizador>> GetAllAsync();
        Task<Utilizador> CreateAsync(Utilizador utilizador);
        Task UpdateAsync(Utilizador utilizador);
        Task DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
