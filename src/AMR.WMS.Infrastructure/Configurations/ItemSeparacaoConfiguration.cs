using AMR.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.WMS.Infrastructure.Configurations;

public class ItemSeparacaoConfiguration : IEntityTypeConfiguration<ItemSeparacao>
{
    public void Configure(EntityTypeBuilder<ItemSeparacao> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.OrdemSeparacaoId).IsRequired();
        b.Property(x => x.ProdutoId).IsRequired();
        b.Property(x => x.LocalizacaoId).IsRequired(false);
        b.Property(x => x.QntSolicitada).IsRequired();
        b.Property(x => x.QntSeparada).IsRequired();

        b.HasIndex(x => x.OrdemSeparacaoId);

        b.HasOne(x => x.Localizacao)
         .WithMany()
         .HasForeignKey(x => x.LocalizacaoId)
         .OnDelete(DeleteBehavior.SetNull)
         .IsRequired(false);
    }
}
