using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Data;

public static class AmrWmsSeed
{
    public static async Task AplicarAsync(AmrWmsDbContext ctx)
    {
        if (await ctx.Localizacoes.AnyAsync()) return;

        var localizacoes = new List<Localizacao>();

        // Zona A — Picking (10 localizações)
        foreach (var corredor in new[] { "01", "02" })
        foreach (var prateleira in new[] { "A", "B", "C" })
        foreach (var posicao in new[] { "01", "02" })
        {
            if (localizacoes.Count(x => x.Zona == "A") >= 10) break;
            localizacoes.Add(Localizacao.Criar("A", corredor, prateleira, posicao, capacidade: 50, TipoLocalizacao.Picking));
        }

        // Zona B — Reserva (10 localizações)
        foreach (var corredor in new[] { "01", "02" })
        foreach (var prateleira in new[] { "A", "B", "C" })
        foreach (var posicao in new[] { "01", "02" })
        {
            if (localizacoes.Count(x => x.Zona == "B") >= 10) break;
            localizacoes.Add(Localizacao.Criar("B", corredor, prateleira, posicao, capacidade: 100, TipoLocalizacao.Reserva));
        }

        // Zona C — Expedição (10 localizações)
        foreach (var corredor in new[] { "01", "02" })
        foreach (var prateleira in new[] { "A", "B", "C" })
        foreach (var posicao in new[] { "01", "02" })
        {
            if (localizacoes.Count(x => x.Zona == "C") >= 10) break;
            localizacoes.Add(Localizacao.Criar("C", corredor, prateleira, posicao, capacidade: 200, TipoLocalizacao.Expedicao));
        }

        ctx.Localizacoes.AddRange(localizacoes);
        await ctx.SaveChangesAsync();
    }
}
