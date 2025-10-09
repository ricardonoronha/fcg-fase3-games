using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Repository;

namespace FIAP.MicroService.Jogos.Infraestrutura.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly JogosDbContext _context;
        
        public JogoRepository(JogosDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Jogo>> GetAllAsync()
        {
            return await _context.Jogos.AsNoTracking()
                                       .ToListAsync();
        }

        public async Task<Jogo?> GetByIdAsync(Guid id)
        {
            return await _context.Jogos.AsNoTracking()
                                       .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Guid> AddAsync(Jogo jogo)
        {
            await _context.Jogos.AddAsync(jogo);
            await _context.SaveChangesAsync();

            return jogo.Id;
        }

        public async Task<Jogo> UpdateAsync(Jogo jogo)
        {
            _context.Jogos.Update(jogo);
            await _context.SaveChangesAsync();

            return jogo;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null) 
                return false;

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}