using AMR.WMS.Application.Separacao.Queries;

namespace AMR.WMS.Application.Separacao.Services;

public interface IAmrCoreService
{
    Task<IReadOnlyList<PedidoVendaDto>> GetPedidosVendaAprovadosAsync(CancellationToken ct = default);
}
