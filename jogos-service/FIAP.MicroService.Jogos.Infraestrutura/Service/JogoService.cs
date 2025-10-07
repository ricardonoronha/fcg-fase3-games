using FIAP.MicroService.Jogos.Dominio;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using OpenSearch.Client;

namespace FIAP.MicroService.Jogos.Infraestrutura.Service;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IOpenSearchClient _openSearchClient;
    
    public JogoService(IJogoRepository jogoRepository, IOpenSearchClient openSearchClient)
    {
        _jogoRepository = jogoRepository;
        _openSearchClient = openSearchClient;
    }

    public Task<IEnumerable<Jogo>> GetAllAsync() => _jogoRepository.GetAllAsync();
    public Task<Jogo> GetByIdAsync(Guid gameId) => _jogoRepository.GetByIdAsync(gameId);
    public Task AddAsync(Jogo jogo) => _jogoRepository.AddAsync(jogo);
    public Task UpdateAsync(Jogo jogo) => _jogoRepository.UpdateAsync(jogo);
    public Task DeleteAsync(Guid id) => _jogoRepository.DeleteAsync(id);
    
    public async Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize)
    {
        var from = (page - 1) * pageSize;

        var response = await _openSearchClient.SearchAsync<Jogo>(s => s
            .Index("jogos")
            .From(from)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm 
                    .Query(query)
                    .Fields(f => f.Field(p => p.Nome).Field(p => p.Categoria)) // Campos a serem buscados
                    .Fuzziness(Fuzziness.Auto) // Permite pequenas variações/erros de digitação
                )
            )
        );

        return new ResultadoBusca<Jogo>
        {
            TotalResultados = response.Total,
            PaginaAtual = page,
            TamanhoPagina = pageSize,
            Itens = response.Documents
        };
    }

    public async Task<ResultadoAgregado> GetTopAggregatesAsync(string by, string window)
    {
        var fieldToAggregate = by.ToLowerInvariant() + ".keyword";

        var response = await _openSearchClient.SearchAsync<Jogo>(s => s
            .Index("jogos")
            .Size(0) 
            .Aggregations(a => a
                .Terms("top_values", t => t
                    .Field(fieldToAggregate)
                    .Size(10) 
                )
            )
        );
        
        var resultado = new ResultadoAgregado();
        
    
        await Task.CompletedTask;
        return resultado;
    }
}