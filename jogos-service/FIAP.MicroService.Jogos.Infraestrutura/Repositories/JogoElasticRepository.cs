using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Dominio.Models;
using Nest;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.MicroService.Jogos.Infraestrutura.Repositories
{
    public class JogoElasticRepository : IJogoElasticRepository
    {
        private readonly IElasticClient _elasticClient;

        public JogoElasticRepository(IElasticClient elasticClient)
        {
            this._elasticClient = elasticClient;
        }

        public async Task IndexarJogo(Jogo jogo)
        {
            var response = await _elasticClient.IndexDocumentAsync(jogo);
            if (!response.IsValid)
                Log.Error("Erro ao indexar jogo: {Erro}", response.OriginalException?.Message);
        }

        public async Task<List<Jogo>> BuscarJogos(string termo)
        {
            var response = await _elasticClient.SearchAsync<Jogo>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Nome)
                        .Query(termo)
                    )
                )
            );

            if (!response.IsValid)
            {
                Log.Error("Erro ao buscar jogos: {Erro}", response.OriginalException?.Message);
                return new List<Jogo>();
            }

            return response.Documents.ToList();
        }

        public async Task AtualizarJogo(Jogo jogo)
        {
            var response = await _elasticClient.IndexDocumentAsync(jogo);
            if (!response.IsValid)
                Log.Error("Erro ao atualizar jogo: {Erro}", response.OriginalException?.Message);
        }

        public async Task ExcluirJogo(Guid id)
        {
            var response = await _elasticClient.DeleteAsync<Jogo>(id);
            if (!response.IsValid)
                Log.Error("Erro ao excluir jogo: {Erro}", response.OriginalException?.Message);
        }
    }
}