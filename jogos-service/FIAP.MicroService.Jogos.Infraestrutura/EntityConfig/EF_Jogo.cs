using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.MicroService.Jogos.Infraestrutura.EntityConfig
{
    public class EF_Jogo : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Categoria)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Preco)
                .HasPrecision(10, 2);
        }
    }
}
