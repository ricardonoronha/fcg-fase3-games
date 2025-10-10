using FIAP.MicroService.Jogos.Dominio.Dtos;
using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces.Service
{
    public interface IJogoService
    {
        Task<IEnumerable<Jogo>> GetAllAsync();
        Task<Jogo?> GetByIdAsync(Guid jogoId);
        Task<Guid> AddAsync(Jogo jogo);
        Task<Jogo> UpdateAsync(Jogo jogo);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ResultadoAgregado>> MostPopularGamesAsync(int top);
        Task<IEnumerable<Jogo>> SuggestGamesAsync(IEnumerable<string> categoriaHistorico, int tamanho);
        Task<PagedResult<Jogo>?> SearchAsync(string textSearch, int page, int pageSize);
    }
}
