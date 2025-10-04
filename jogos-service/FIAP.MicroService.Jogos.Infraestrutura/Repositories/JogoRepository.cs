using FIAP.MicroService.Jogos.Dominio;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Data; 
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks;

namespace FIAP.MicroService.Jogos.Infraestrutura.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly JogosDbContext _context; 

        // 1. O construtor agora recebe o DbContext via DI.
        public JogoRepository(JogosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Jogo>> GetAllAsync()
        {
            // Usa o EF Core para buscar todos os registros na tabela Jogos.
            return await _context.Jogos.ToListAsync(); 
        }

        public async Task<Jogo> GetByIdAsync(Guid id)
        {
            // Usa o EF Core para buscar o registro por Id.
            return await _context.Jogos.FindAsync(id); 
        }

        public async Task AddAsync(Jogo jogo)
        {
            await _context.Jogos.AddAsync(jogo);
            await _context.SaveChangesAsync(); // Persiste a mudança no banco de dados.
        }

        public async Task UpdateAsync(Jogo jogoAtualizado)
        {
            _context.Jogos.Update(jogoAtualizado);
            await _context.SaveChangesAsync(); // Persiste a mudança no banco.
        }

        public async Task DeleteAsync(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync(); // Persiste a mudança no banco.
            }
        }
    }
}