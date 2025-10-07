using Serilog;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using OpenSearch.Client;

namespace FIAP.MicroService.Jogos.Dominio.Service
{

    public class JogoService : IJogoService
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IOpenSearchClient _openSearchClient;
        private readonly IJogoElasticRepository _elasticRepository;

        public JogoService(IJogoRepository jogoRepository, IJogoElasticRepository elasticRepository, IOpenSearchClient openSearchClient)
        {
            this._jogoRepository = jogoRepository;
            this._openSearchClient = openSearchClient;
            this._elasticRepository = elasticRepository;
        }

        public async Task<List<Jogo>> ObtenhaTodosJogos()
        {
            return await this._jogoRepository.ObtenhaTodosJogos();
        }

        public async Task<Jogo> ObtenhaJogoPorId(Guid jogoId)
        {
            if (jogoId == Guid.Empty)
            {
                Log.Warning("É necessário colocar um ID para prosseguir com a operação!");
                throw new ArgumentException("É necessário colocar um ID para prosseguir com a operação!", nameof(jogoId));
            }

            try
            {
                var jogo = await _jogoRepository.ObtenhaJogoPorId(jogoId);

                if (jogo == null)
                {
                    Log.Information("O jogo não foi encontrado!");
                    return null;
                }

                return jogo;
            }
            catch (Exception ex)
            {
                Log.Error($"Erro ao buscar o jogo por Id: {ex.Message}");
                throw;
            }
        }

        public async Task<Guid> CriarJogo(Jogo jogo)
        {
            if (jogo == null)
            {
                Log.Warning("É necessário dos dados do jogo para cadastrar!");
                throw new ArgumentException("É necessário dos dados do jogo!", nameof(jogo));
            }

            try
            {
                var jogoNovo = new Jogo
                {
                    Id = Guid.NewGuid(),
                    Nome = jogo.Nome,
                    Categoria = jogo.Categoria,
                    Preco = jogo.Preco,
                    DataLancamento = jogo.DataLancamento
                };

                await _jogoRepository.CriarJogo(jogoNovo);

                await _elasticRepository.IndexarJogo(jogoNovo);
                Log.Information("Jogo {JogoId} criado e indexado com sucesso!", jogoNovo.Id);

                return jogoNovo.Id;
            } catch (Exception ex)
            {
                Log.Error(ex, "Erro ao criar o jogo");
                throw;
            }
        }

        public async Task<Jogo> AtualizarJogo(Jogo jogo)
        {
            if (jogo == null)
            {
                Log.Warning("É necessário fornecer os dados do jogo para atualizar!");
                throw new ArgumentException("Dados inválidos!", nameof(jogo));
            }

            try
            {
                var jogoExistente = await _jogoRepository.ObtenhaJogoPorId(jogo.Id);

                if (jogoExistente == null)
                {
                    Log.Warning("Tentativa de atualizar jogo inexistente: {JogoId}", jogo.Id);
                    throw new ArgumentException("O jogo não existe!");
                }

                jogoExistente.Nome = jogo.Nome;
                jogoExistente.Categoria = jogo.Categoria;
                jogoExistente.Preco = jogo.Preco;
                jogoExistente.DataLancamento = jogo.DataLancamento;

                await _jogoRepository.AtualizarJogo(jogoExistente);

                await _elasticRepository.AtualizarJogo(jogoExistente);
                Log.Information("Jogo {JogoId} atualizado e indexado com sucesso!", jogoExistente.Id);

                return jogoExistente;
            } catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar o jogo");
                throw;
            }
        }

        public async Task<bool> ExcluirJogo(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Warning("ID inválido para exclusão!");
                throw new ArgumentException("ID inválido", nameof(id));
            }

            try
            {
                var jogo = await _jogoRepository.ObtenhaJogoPorId(id);
                if (jogo == null)
                {
                    Log.Warning("Tentativa de excluir jogo inexistente: {JogoId}", id);
                    throw new ArgumentException("O jogo não existe!");
                }

                await _jogoRepository.ExcluirJogo(id);

                await _elasticRepository.ExcluirJogo(id);
                Log.Information("Jogo {JogoId} excluído com sucesso do banco e Elasticsearch", id);

                return true;
            } catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir o jogo");
                throw;
            }
        }

        public async Task<ResultadoBusca<Jogo>> SearchGamesAsync(string query, int page, int pageSize)
        {
            try
            {
                var resultados = await _elasticRepository.BuscarJogos(query);

                var itensPaginados = resultados
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ResultadoBusca<Jogo>
                {
                    TotalResultados = resultados.Count,
                    PaginaAtual = page,
                    TamanhoPagina = pageSize,
                    Itens = itensPaginados
                };
            } catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar jogos no Elasticsearch");
                throw;
            }
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
