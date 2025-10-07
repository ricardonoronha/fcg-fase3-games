using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces
{
    public interface IJogoElasticRepository
    {
        Task IndexarJogo(Jogo jogo);
        Task<List<Jogo>> BuscarJogos(string termo);
        Task AtualizarJogo(Jogo jogo);
        Task ExcluirJogo(Guid id);
    }
}
