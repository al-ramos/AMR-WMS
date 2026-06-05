using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;
using AMR.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Repositories;

public class OrdemSeparacaoRepository(AmrWmsDbContext ctx) : IOrdemSeparacaoRepository
{
    public async Task<OrdemSeparacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await ctx.OrdensSeparacao
            .Include(o => o.Itens)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<OrdemSeparacao>> ListarAsync(CancellationToken ct = default)
        => await ctx.OrdensSeparacao
            .Include(o => o.Itens)
            .OrderByDescending(o => o.DataCriacao)
            .ToListAsync(ct);

    public async Task AdicionarAsync(OrdemSeparacao ordem, CancellationToken ct = default)
        => await ctx.OrdensSeparacao.AddAsync(ordem, ct);

    public void Atualizar(OrdemSeparacao ordem)
        => ctx.OrdensSeparacao.Update(ordem);
}
