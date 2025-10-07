using FIAP.MicroService.Jogos.Dominio;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;

using OpenSearch.Client; 

namespace FIAP.MicroService.Jogos.Infraestrutura.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly JogosDbContext _context;
        private readonly IOpenSearchClient _openSearchClient; 
        
        public JogoRepository(JogosDbContext context, IOpenSearchClient openSearchClient)
        {
            _context = context;
            _openSearchClient = openSearchClient;
        }

        public async Task<IEnumerable<Jogo>> GetAllAsync()
        {
            return await _context.Jogos.ToListAsync();
        }

        public async Task<Jogo> GetByIdAsync(Guid id)
        {
            return await _context.Jogos.FindAsync(id);
        }

        public async Task AddAsync(Jogo jogo)
        {
        
            await _context.Jogos.AddAsync(jogo);
            await _context.SaveChangesAsync();
            
            await _openSearchClient.IndexAsync(jogo, idx => idx.Index("jogos"));
        }

        public async Task UpdateAsync(Jogo jogoAtualizado)
        {
            _context.Jogos.Update(jogoAtualizado);
            await _context.SaveChangesAsync();

           
            await _openSearchClient.IndexAsync(jogoAtualizado, idx => idx.Index("jogos"));
        }

        public async Task DeleteAsync(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync();
                
                var deleteRequest = new DeleteRequest("jogos", id.ToString());
                
                await _openSearchClient.DeleteAsync(deleteRequest);
            }
        
        }
    }
}