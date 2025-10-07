namespace FIAP.MicroService.Jogos.Dominio.Interfaces;

public interface IJogoService
{
    // --- Métodos CRUD ---
    Task<IEnumerable<Jogo>> GetAllAsync();
    Task<Jogo> GetByIdAsync(Guid gameId);
    Task AddAsync(Jogo jogo);
    Task UpdateAsync(Jogo jogo);
    Task DeleteAsync(Guid id);
    
    Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize);
    
    Task<ResultadoAgregado> GetTopAggregatesAsync(string by, string window);
}