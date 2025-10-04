namespace FIAP.MicroService.Jogos.Dominio.Interfaces
{
    public interface IJogoService
    {
        Task<Jogo> GetByIdAsync(Guid gameId);
        
        // Busca paginada no Elasticsearch
        Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize);
        
        // Consulta agregada (simulada)
        Task<IEnumerable<ResultadoAgregado>> GetTopAggregatesAsync(string by, string window);

        // Funções CRUD básicas (mantidas, podem delegar para o Repositório)
        Task<IEnumerable<Jogo>> GetAllAsync();
        Task AddAsync(Jogo jogo);
        Task UpdateAsync(Jogo jogo);
        Task DeleteAsync(Guid id);
    }
}