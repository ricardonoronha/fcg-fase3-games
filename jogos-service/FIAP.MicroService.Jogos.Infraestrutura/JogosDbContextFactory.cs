using FIAP.MicroService.Jogos.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FIAP.MicroService.Jogos.Infraestrutura
{
    public class JogosDbContextFactory : IDesignTimeDbContextFactory<JogosDbContext>
    {
        public JogosDbContext CreateDbContext(string[] args)
        {
            // O EF Core precisa ler o appsettings.json da API
            IConfigurationRoot configuration = new ConfigurationBuilder()
                
                .SetBasePath(Directory.GetCurrentDirectory()) 
                
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<JogosDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configuração de SqlLite
            optionsBuilder.UseSqlite(connectionString);

            // Retorna o novo contexto criado
            return new JogosDbContext(optionsBuilder.Options);
        }
    }
}