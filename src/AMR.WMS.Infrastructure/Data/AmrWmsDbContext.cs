using Microsoft.EntityFrameworkCore;
using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;

namespace AMR.WMS.Infrastructure.Data;

public class AmrWmsDbContext(DbContextOptions<AmrWmsDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Localizacao>         Localizacoes     => Set<Localizacao>();
    public DbSet<MovimentacaoEstoque> Movimentacoes    => Set<MovimentacaoEstoque>();
    public DbSet<OrdemSeparacao>      OrdensSeparacao  => Set<OrdemSeparacao>();
    public DbSet<ItemSeparacao>       ItensSeparacao   => Set<ItemSeparacao>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(AmrWmsDbContext).Assembly);
        base.OnModelCreating(mb);
    }
}
