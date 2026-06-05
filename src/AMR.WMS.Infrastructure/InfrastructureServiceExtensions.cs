using AMR.WMS.Domain.Interfaces;
using AMR.WMS.Infrastructure.Data;
using AMR.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AMR.WMS.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AmrWmsDbContext>(opts =>
            opts.UseSqlite(config.GetConnectionString("AmrWms")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AmrWmsDbContext>());
        services.AddScoped<ILocalizacaoRepository, LocalizacaoRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

        return services;
    }
}
