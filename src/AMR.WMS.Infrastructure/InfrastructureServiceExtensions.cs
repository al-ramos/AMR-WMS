using AMR.WMS.Application.Dashboard;
using AMR.WMS.Application.Separacao.Services;
using AMR.WMS.Domain.Interfaces;
using AMR.WMS.Infrastructure.Data;
using AMR.WMS.Infrastructure.Repositories;
using AMR.WMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AMR.WMS.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddDbContext<AmrWmsDbContext>(opts =>
            opts.UseSqlite(config.GetConnectionString("AmrWms")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AmrWmsDbContext>());
        services.AddScoped<ILocalizacaoRepository, LocalizacaoRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
        services.AddScoped<IOrdemSeparacaoRepository, OrdemSeparacaoRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        services.AddHttpClient<IAmrCoreService, AmrCoreService>(client =>
            client.BaseAddress = UrlDoServico(config, environment, "AmrCore:BaseUrl", "http://localhost:5001/"));

        return services;
    }

    // Resolve a URL de um servico integrado. Fora de Development a URL tem de vir
    // da configuracao: um default de localhost dentro de um container significa
    // integracao que falha em runtime, e nao no boot, onde da para ver.
    private static Uri UrlDoServico(IConfiguration cfg, IHostEnvironment env, string chave, string urlDeDesenvolvimento)
    {
        var valor = cfg[chave];
        if (!string.IsNullOrWhiteSpace(valor))
            return new Uri(valor);

        if (env.IsDevelopment())
            return new Uri(urlDeDesenvolvimento);

        throw new InvalidOperationException(
            $"Configuracao obrigatoria ausente: '{chave}'. " +
            $"Defina a variavel de ambiente '{chave.Replace(":", "__")}' com a URL do servico.");
    }
}
