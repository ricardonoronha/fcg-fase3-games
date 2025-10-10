using Serilog;
using FluentValidation;
using OpenSearch.Client;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Service;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FIAP.MicroService.Jogos.Dominio.Dtos;

namespace FIAP.MicroService.Jogos.Dominio.Service
{
    public class JogoService : IJogoService
    {
        private readonly IValidator<Jogo> _validator;
        private readonly IJogoRepository _jogoRepository;
        private readonly IOpenSearchClient _openSearchClient;

        public JogoService(IValidator<Jogo> validator, IJogoRepository jogoRepository, IOpenSearchClient openSearchClient)
        {
            this._validator = validator;
            this._jogoRepository = jogoRepository;
            this._openSearchClient = openSearchClient;
        }

        public async Task<IEnumerable<Jogo>> GetAllAsync()
        {
            try
            {
                var jogos = await this._jogoRepository.GetAllAsync();

                return jogos;
            }
            catch (Exception ex)
            {
                Log.Error($"Falha na obtenção de todos os jogos: {ex}");
                throw;
            }
        }

        public async Task<Jogo?> GetByIdAsync(Guid jogoId)
        {
            try
            {
                var jogo = await this._jogoRepository.GetByIdAsync(jogoId);

                if (jogo == null)
                {
                    Log.Warning("O jogo não pode ser encontrado ou não existe!");
                }

                return jogo;
            }
            catch (Exception ex)
            {
                Log.Error($"Falha na obtenção do jogo: {ex}");
                throw;
            }
        }

        public async Task<Guid> AddAsync(Jogo jogo)
        {
            var validationResult = _validator.Validate(jogo);

            if (!validationResult.IsValid)
            {
                Log.Warning($"Falha na validação ao criar jogo: {validationResult.Errors}");
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var jogoNovo = await this._jogoRepository.AddAsync(jogo);
                var response = await _openSearchClient.IndexAsync(jogo, idx => idx.Index("jogos"));

                if (!response.IsValid)
                {
                    Log.Warning($"Falha ao indexar jogo no OpenSearch: {response.OriginalException?.Message}");
                }

                return jogo.Id;
            }
            catch (Exception ex)
            {
                Log.Error($"Falha na criação do jogo: {ex}");
                throw;
            }
        }

        public async Task<Jogo> UpdateAsync(Jogo jogo)
        {
            try
            {
                var jogoExiste = await this._jogoRepository.GetByIdAsync(jogo.Id);

                if (jogoExiste == null)
                {
                    Log.Warning("O jogo não pode ser encontrado ou não existe!");
                    return null;
                }

                var jogoAtualizar = await this._jogoRepository.UpdateAsync(jogo);
                var response = await _openSearchClient.UpdateAsync<Jogo>(jogo.Id, u => u.Index("jogos").Doc(jogo));

                if (!response.IsValid)
                {
                    Log.Warning("Falha ao atualizar índice do jogo no OpenSearch!");
                }

                return jogoAtualizar;
            }
            catch (Exception ex)
            {
                Log.Error($"Falha na atualização do jogo: {ex}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var jogo = await this._jogoRepository.GetByIdAsync(id);

                if (jogo == null)
                {
                    Log.Warning("O jogo não pode ser encontrado ou não existe!");
                    return false;
                }

                var jogoExcluido = await this._jogoRepository.DeleteAsync(id);

                if (!jogoExcluido)
                {
                    Log.Warning("Falha na tentativa de exclusão do jogo!");
                    return false;
                }

                var response = await _openSearchClient.DeleteAsync<Jogo>(id, d => d.Index("jogos"));

                if (!response.IsValid)
                {
                    Log.Warning("Falha ao excluir índice do jogo no OpenSearch!");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Falha na exclusão do jogo: {ex}");
                throw;
            }
        }

        public async Task<PagedResult<Jogo>?> SearchAsync(string textSearch, int page, int pageSize)
        {
            var response = await _openSearchClient.SearchAsync<Jogo>(s => s
                .Index("jogos")
                .From((page - 1) * pageSize) 
                .Size(pageSize)              
                .Query(q => q
                    .Wildcard(w => w
                        .Field("nome.keyword")
                        .Value($"*{textSearch.ToLower()}*")
                        .CaseInsensitive(true)
                    )
                )
            );

            if (!response.IsValid)
                return null;

            return new PagedResult<Jogo>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = response.Total,
                Items = response.Documents
            };
        }

        public async Task<IEnumerable<ResultadoAgregado>> MostPopularGamesAsync(int top = 5)
        {
            var response = await this._openSearchClient.SearchAsync<Jogo>(s => s.Size(0)
                .Aggregations(
                    a => a.Terms(
                        "mais_populares", t => t.Field(
                            f => f.Nome.Suffix("keyword"))
                        .Size(top)
                    )
                )
            );

            if (!response.IsValid)
            {
                Log.Error($"Erro ao agregar os jogos mais populares: {response.OriginalException?.Message}");
                return Enumerable.Empty<ResultadoAgregado>();
            }

            var buckets = response.Aggregations.Terms("mais_populares")?.Buckets;

            return buckets.Select(b => new ResultadoAgregado
            {
                Chave = b.Key,
                TotalVendas = (int)b.DocCount
            });
        }

        public async Task<IEnumerable<Jogo>> SuggestGamesAsync(IEnumerable<string> categoriaHistorico, int tamanho = 5)
        {
            var response = await _openSearchClient.SearchAsync<Jogo>(s => s.Query(
                q => q.Terms(
                    t => t.Field(
                        f => f.Categoria.Suffix("keyword"))
                    .Terms(categoriaHistorico)))
                .Size(tamanho)
            );

            if (!response.IsValid)
            {
                Log.Error($"Erro ao buscar sugestões: {response.OriginalException?.Message}");
                return Enumerable.Empty<Jogo>();
            }

            return response.Documents;
        }
    }
}
