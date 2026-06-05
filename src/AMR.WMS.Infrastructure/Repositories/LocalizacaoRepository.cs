using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;
using AMR.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Repositories;

public class LocalizacaoRepository(AmrWmsDbContext ctx) : ILocalizacaoRepository
{
    public async Task<Localizacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await ctx.Localizacoes.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Localizacao?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default)
        => await ctx.Localizacoes.FirstOrDefaultAsync(x => x.Codigo == codigo, ct);

    public async Task<IReadOnlyList<Localizacao>> ListarAsync(CancellationToken ct = default)
        => await ctx.Localizacoes.OrderBy(x => x.Zona).ThenBy(x => x.Corredor).ThenBy(x => x.Prateleira).ThenBy(x => x.Posicao).ToListAsync(ct);

    public async Task<IReadOnlyList<Localizacao>> ListarDisponiveisAsync(CancellationToken ct = default)
        => await ctx.Localizacoes
            .Where(x => x.Ocupacao < x.Capacidade)
            .OrderBy(x => x.Zona).ThenBy(x => x.Corredor).ThenBy(x => x.Prateleira).ThenBy(x => x.Posicao)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Localizacao>> ListarPorZonaAsync(string zona, CancellationToken ct = default)
        => await ctx.Localizacoes
            .Where(x => x.Zona == zona.ToUpper())
            .OrderBy(x => x.Corredor).ThenBy(x => x.Prateleira).ThenBy(x => x.Posicao)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Localizacao localizacao, CancellationToken ct = default)
        => await ctx.Localizacoes.AddAsync(localizacao, ct);

    public void Atualizar(Localizacao localizacao)
        => ctx.Localizacoes.Update(localizacao);

    public void Remover(Localizacao localizacao)
        => ctx.Localizacoes.Remove(localizacao);
}
