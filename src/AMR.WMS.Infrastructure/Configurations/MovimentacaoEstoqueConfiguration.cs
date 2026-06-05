using AMR.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.WMS.Infrastructure.Configurations;

public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ProdutoId).IsRequired();
        b.Property(x => x.LocalizacaoId).IsRequired();
        b.Property(x => x.Tipo).IsRequired();
        b.Property(x => x.Quantidade).IsRequired();
        b.Property(x => x.DataHora).IsRequired();
        b.Property(x => x.Observacao).HasMaxLength(500);
    }
}
