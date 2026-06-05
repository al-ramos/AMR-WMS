using AMR.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.WMS.Infrastructure.Configurations;

public class LocalizacaoConfiguration : IEntityTypeConfiguration<Localizacao>
{
    public void Configure(EntityTypeBuilder<Localizacao> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.Codigo).IsUnique();
        b.Property(x => x.Zona).IsRequired().HasMaxLength(10);
        b.Property(x => x.Corredor).IsRequired().HasMaxLength(10);
        b.Property(x => x.Prateleira).IsRequired().HasMaxLength(10);
        b.Property(x => x.Posicao).IsRequired().HasMaxLength(10);
        b.Property(x => x.Capacidade).IsRequired();
        b.Property(x => x.Ocupacao).IsRequired();
        b.Property(x => x.TipoLocalizacao).IsRequired();
        b.Property(x => x.CriadoEm).IsRequired();
        b.Property(x => x.AlteradoEm).IsRequired();

        b.HasMany(x => x.Movimentacoes)
         .WithOne(x => x.Localizacao)
         .HasForeignKey(x => x.LocalizacaoId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
