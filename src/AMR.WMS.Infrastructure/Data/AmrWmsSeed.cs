using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AMR.WMS.Infrastructure.Data;

public static class AmrWmsSeed
{
    public static async Task AplicarAsync(AmrWmsDbContext ctx)
    {
        await SeedLocalizacoes(ctx);
        await SeedOrdensSeparacao(ctx);
    }

    private static async Task SeedLocalizacoes(AmrWmsDbContext ctx)
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

    private static async Task SeedOrdensSeparacao(AmrWmsDbContext ctx)
    {
        if (await ctx.OrdensSeparacao.AnyAsync()) return;

        var ordens = new[]
        {
            OrdemSeparacao.Criar(1001, [(101, 5), (102, 3)]),
            OrdemSeparacao.Criar(1002, [(103, 10)]),
            OrdemSeparacao.Criar(1003, [(104, 2), (105, 8), (106, 4)]),
        };

        ordens[1].IniciarSeparacao();
        ordens[2].IniciarSeparacao();
        ordens[2].Concluir();

        ctx.OrdensSeparacao.AddRange(ordens);
        await ctx.SaveChangesAsync();
    }
}
