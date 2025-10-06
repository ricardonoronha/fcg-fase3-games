using FIAP.MicroService.Jogos.Dominio;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using OpenSearch.Client;

namespace FIAP.MicroService.Jogos.Infraestrutura.Service
{

    public class JogoService(IJogoRepository repository, OpenSearchClient openSearchClient) : IJogoService
    {
        public Task<Jogo> GetByIdAsync(Guid gameId) => repository.GetByIdAsync(gameId);
        public Task<IEnumerable<Jogo>> GetAllAsync() => repository.GetAllAsync();
        public Task AddAsync(Jogo jogo) => repository.AddAsync(jogo);
        public Task UpdateAsync(Jogo jogo) => repository.UpdateAsync(jogo);
        public Task DeleteAsync(Guid id) => repository.DeleteAsync(id);

        public async Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize)
        {
            await openSearchClient.PingAsync();

            var allGames = await repository.GetAllAsync();
            
            // Filtra por Nome ou Categoria que contenha a query
            var filteredGames = allGames.Where(j => 
                j.Nome.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                j.Categoria.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            var itensPaginados = filteredGames
                .Skip((page - 1) * pageSize)
                
                .Take(pageSize)
                .ToList();

            return new ResultadoBusca<Jogo>
            {
                TotalResultados = filteredGames.Count,
                PaginaAtual = page,
                TamanhoPagina = pageSize,
                Itens = itensPaginados
            };
        }

        public Task<IEnumerable<ResultadoAgregado>> GetTopAggregatesAsync(string by, string window)
        {
            
            var resultados = new List<ResultadoAgregado>();

            if (by.Equals("genre", StringComparison.OrdinalIgnoreCase))
            {
                resultados.Add(new ResultadoAgregado { Chave = "RPG", ReceitaTotal = 15000.00m, TotalVendas = 150 });
                resultados.Add(new ResultadoAgregado { Chave = "Ação", ReceitaTotal = 9500.50m, TotalVendas = 200 });
            }
            else if (by.Equals("game", StringComparison.OrdinalIgnoreCase))
            {
                resultados.Add(new ResultadoAgregado { Chave = "The Witcher 3", ReceitaTotal = 8000.00m, TotalVendas = 70 });
                resultados.Add(new ResultadoAgregado { Chave = "Cyberpunk 2077", ReceitaTotal = 7000.00m, TotalVendas = 80 });
            }

            return Task.FromResult<IEnumerable<ResultadoAgregado>>(resultados);
        }
    }
}
