using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces
{
    public interface IJogoService
    {
        Task<List<Jogo>> ObtenhaTodosJogos();
        Task<Jogo> ObtenhaJogoPorId(Guid jogoId);
        Task<Guid> CriarJogo(Jogo jogo);
        Task<Jogo> AtualizarJogo(Jogo jogo);
        Task<bool> ExcluirJogo(Guid id);
        
        Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize);
        
        // Consulta agregada (simulada)
        Task<IEnumerable<ResultadoAgregado>> GetTopAggregatesAsync(string by, string window);
    }
}