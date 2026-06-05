using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;
using AMR.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Repositories;

public class MovimentacaoRepository(AmrWmsDbContext ctx) : IMovimentacaoRepository
{
    public async Task<IReadOnlyList<MovimentacaoEstoque>> ListarPorLocalizacaoAsync(Guid localizacaoId, CancellationToken ct = default)
        => await ctx.Movimentacoes
            .Where(x => x.LocalizacaoId == localizacaoId)
            .OrderByDescending(x => x.DataHora)
            .ToListAsync(ct);

    public async Task AdicionarAsync(MovimentacaoEstoque movimentacao, CancellationToken ct = default)
        => await ctx.Movimentacoes.AddAsync(movimentacao, ct);
}
