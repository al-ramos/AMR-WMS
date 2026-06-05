using System.Net.Http.Json;
using AMR.WMS.Application.Separacao.Queries;
using AMR.WMS.Application.Separacao.Services;

namespace AMR.WMS.Infrastructure.Services;

public class AmrCoreService(HttpClient http) : IAmrCoreService
{
    public async Task<IReadOnlyList<PedidoVendaDto>> GetPedidosVendaAprovadosAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await http.GetFromJsonAsync<List<PedidoVendaDto>>(
                "api/pedidos-venda/aprovados", ct);
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }
}
