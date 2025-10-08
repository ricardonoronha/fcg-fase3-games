using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Infraestrutura.Data
{
    public class JogosDbContext : DbContext
    {
        public JogosDbContext(DbContextOptions<JogosDbContext> options) : base(options) { }

        public DbSet<Jogo> Jogos { get; set; }
    }
}