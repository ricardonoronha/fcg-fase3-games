using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIAP.MicroService.Jogos.Dominio.Interfaces
{
    public interface IJogoRepository
    {
        Task<IEnumerable<Jogo>> GetAllAsync();
        Task<Jogo> GetByIdAsync(Guid id); 
        Task AddAsync(Jogo jogo);
        Task UpdateAsync(Jogo jogo);
        Task DeleteAsync(Guid id);
    }
}