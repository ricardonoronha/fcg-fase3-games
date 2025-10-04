using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio;

namespace FIAP.MicroService.Jogos.Infraestrutura.Data
{
    // A classe deve herdar de DbContext
    public class JogosDbContext : DbContext
    {
        // Construtor obrigatório para Injeção de Dependência
        public JogosDbContext(DbContextOptions<JogosDbContext> options)
            : base(options)
        {
        }

        // Mapeia a classe Jogo para a tabela "Jogos" no banco de dados.
        public DbSet<Jogo> Jogos { get; set; } = default!;
    }
}