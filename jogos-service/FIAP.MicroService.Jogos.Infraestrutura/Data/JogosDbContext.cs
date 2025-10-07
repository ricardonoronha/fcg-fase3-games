using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Infraestrutura.Data
{
    public class DbContextJogos : DbContext
    {
        public DbContextJogos(DbContextOptions<DbContextJogos> options) : base(options) { }

        public DbSet<Jogo> Jogos { get; set; } = default!;
    }
}