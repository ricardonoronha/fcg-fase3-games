using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;

namespace FIAP.MicroService.Jogos.Infraestrutura.ConfigEntity
{
    public class EF_Jogos : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Jogo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Preco)
                .HasColumnType("decimal(10,2)");

            builder.ToTable("jogos");
        }
    }
}
