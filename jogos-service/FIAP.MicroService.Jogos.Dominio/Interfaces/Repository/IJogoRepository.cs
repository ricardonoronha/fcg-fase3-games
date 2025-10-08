using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces.Repository
{
    public interface IJogoRepository
    {
        Task<IEnumerable<Jogo>> GetAllAsync();
        Task<Jogo?> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(Jogo jogo);
        Task<Jogo> UpdateAsync(Jogo jogo);
        Task<bool> DeleteAsync(Guid id);
    }
}
