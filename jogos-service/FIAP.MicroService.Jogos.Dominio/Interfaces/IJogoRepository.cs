using FIAP.MicroService.Jogos.Dominio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces
{
    public interface IJogoRepository
    {
        Task<List<Jogo>> ObtenhaTodosJogos();
        Task<Jogo> ObtenhaJogoPorId(Guid jogoId);
        Task<Guid> CriarJogo(Jogo jogo);
        Task<Jogo> AtualizarJogo(Jogo jogo);
        Task<bool> ExcluirJogo(Guid id);
    }
}