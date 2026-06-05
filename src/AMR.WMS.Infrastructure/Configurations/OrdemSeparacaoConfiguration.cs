using AMR.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.WMS.Infrastructure.Configurations;

public class OrdemSeparacaoConfiguration : IEntityTypeConfiguration<OrdemSeparacao>
{
    public void Configure(EntityTypeBuilder<OrdemSeparacao> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PedidoVendaId).IsRequired();
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.DataCriacao).IsRequired();

        b.HasMany(x => x.Itens)
         .WithOne()
         .HasForeignKey(x => x.OrdemSeparacaoId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
