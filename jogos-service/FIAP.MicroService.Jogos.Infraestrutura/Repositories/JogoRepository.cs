using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Data;

namespace FIAP.MicroService.Jogos.Infraestrutura.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly DbContextJogos _jogosDbContext;

        public JogoRepository(DbContextJogos jogosDbContext)
        {
            _jogosDbContext = jogosDbContext;
        }

        public async Task<List<Jogo>> ObtenhaTodosJogos()
        {
            return await _jogosDbContext.Jogos.AsNoTracking()
                                              .ToListAsync();
        }

        public async Task<Jogo> ObtenhaJogoPorId(Guid jogoId)
        {
            return await _jogosDbContext.Jogos.AsNoTracking()
                                              .FirstOrDefaultAsync(j => j.Id == jogoId);
        }

        public async Task<Guid> CriarJogo(Jogo jogo)
        {
            await _jogosDbContext.Jogos.AddAsync(jogo);
            await _jogosDbContext.SaveChangesAsync();
            return jogo.Id;
        }

        public async Task<Jogo> AtualizarJogo(Jogo jogo)
        {
            _jogosDbContext.Jogos.Update(jogo);
            await _jogosDbContext.SaveChangesAsync();
            return jogo;
        }

        public async Task<bool> ExcluirJogo(Guid id)
        {
            var jogo = await _jogosDbContext.Jogos.FindAsync(id);
            if (jogo == null)
                return false;

            _jogosDbContext.Jogos.Remove(jogo);
            await _jogosDbContext.SaveChangesAsync();
            return true;
        }
    }
}