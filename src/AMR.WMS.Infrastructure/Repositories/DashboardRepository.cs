using AMR.WMS.Application.Dashboard;
using AMR.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Repositories;

public class DashboardRepository(AmrWmsDbContext ctx) : IDashboardRepository
{
    public async Task<DashboardWmsKpisDto> GetKpisAsync(int dias, CancellationToken ct = default)
    {
        var locs = await ctx.Localizacoes.ToListAsync(ct);
        var total = locs.Count;
        var mediaOcupacao = total > 0
            ? locs.Average(l => l.Capacidade > 0 ? (double)l.Ocupacao / l.Capacidade * 100 : 0.0)
            : 0.0;
        var alertas = locs.Count(l => l.Capacidade > 0 && (double)l.Ocupacao / l.Capacidade > 0.9);

        var desde = DateTime.UtcNow.AddDays(-dias);
        var ordemsSep = await ctx.OrdensSeparacao
            .CountAsync(o => o.DataCriacao >= desde, ct);

        return new DashboardWmsKpisDto(total, Math.Round(mediaOcupacao, 1), alertas, ordemsSep, 0);
    }

    public async Task<IReadOnlyList<OcupacaoPorZonaDto>> GetOcupacaoPorZonaAsync(CancellationToken ct = default)
    {
        var raw = await ctx.Localizacoes
            .GroupBy(l => l.Zona)
            .Select(g => new
            {
                Zona       = g.Key,
                Total      = g.Count(),
                Capacidade = g.Sum(l => l.Capacidade),
                Ocupacao   = g.Sum(l => l.Ocupacao),
            })
            .OrderBy(z => z.Zona)
            .ToListAsync(ct);

        return raw.Select(z => new OcupacaoPorZonaDto(
            z.Zona,
            z.Total,
            z.Capacidade,
            z.Ocupacao,
            z.Capacidade > 0 ? Math.Round((double)z.Ocupacao / z.Capacidade * 100, 1) : 0.0
        )).ToList();
    }

    public async Task<IReadOnlyList<TopProdutoDto>> GetTopProdutosAsync(int dias, CancellationToken ct = default)
    {
        var desde = DateTime.UtcNow.AddDays(-dias);
        var raw = await ctx.Movimentacoes
            .Where(m => m.DataHora >= desde)
            .GroupBy(m => m.ProdutoId)
            .Select(g => new
            {
                ProdutoId          = g.Key,
                TotalMovimentacoes = g.Count(),
                TotalQuantidade    = g.Sum(m => m.Quantidade),
            })
            .OrderByDescending(x => x.TotalMovimentacoes)
            .Take(10)
            .ToListAsync(ct);

        return raw.Select(x => new TopProdutoDto(x.ProdutoId, x.TotalMovimentacoes, x.TotalQuantidade)).ToList();
    }

    public async Task<IReadOnlyList<AlertaLocalizacaoDto>> GetAlertasAsync(CancellationToken ct = default)
    {
        var locs = await ctx.Localizacoes
            .Where(l => l.Capacidade > 0)
            .ToListAsync(ct);

        return locs
            .Where(l => (double)l.Ocupacao / l.Capacidade > 0.9)
            .Select(l => new AlertaLocalizacaoDto(
                l.Id,
                l.Codigo,
                l.Zona,
                l.Ocupacao,
                l.Capacidade,
                Math.Round((double)l.Ocupacao / l.Capacidade * 100, 1)
            ))
            .OrderByDescending(a => a.OcupacaoPct)
            .ToList();
    }
}
