using NzolaNet.API.DTOs;
using NzolaNet.API.Models;

namespace NzolaNet.API.Interfaces
{
    public interface IPublicacaoRepository
    {
        Task<Publicacao?> GetByIdAsync(int id);
        Task<IEnumerable<PostDto>> GetFeedDtoAsync(int userId, int page, int pageSize);
        Task<IEnumerable<PostDto>> GetByUtilizadorDtoAsync(int utilizadorId, int userId, int page, int pageSize);
        Task<IEnumerable<PostDto>> GetFollowingFeedDtoAsync(int utilizadorId, int page, int pageSize);
        Task<IEnumerable<Publicacao>> GetFeedAsync(int page, int pageSize);
        Task<IEnumerable<Publicacao>> GetByUtilizadorAsync(int utilizadorId, int page, int pageSize);
        Task<IEnumerable<Publicacao>> GetFollowingFeedAsync(int utilizadorId, int page, int pageSize);
        Task<Publicacao> CreateAsync(Publicacao publicacao);
        Task UpdateAsync(Publicacao publicacao);
        Task DeleteAsync(int id);
        Task<int> CountByUtilizadorAsync(int utilizadorId);
    }
}
