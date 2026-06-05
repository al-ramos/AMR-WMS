using AMR.WMS.Application.Separacao.Services;
using MediatR;

namespace AMR.WMS.Application.Separacao.Queries;

public record PedidoVendaDto(
    int    Id,
    string Numero,
    string Cliente,
    DateTime DataAprovacao,
    IReadOnlyList<ItemPedidoVendaDto> Itens
);

public record ItemPedidoVendaDto(
    int ProdutoId,
    string NomeProduto,
    int Quantidade
);

public record GetPVsAprovadosQuery : IRequest<IReadOnlyList<PedidoVendaDto>>;

public class GetPVsAprovadosQueryHandler(IAmrCoreService coreService)
    : IRequestHandler<GetPVsAprovadosQuery, IReadOnlyList<PedidoVendaDto>>
{
    public Task<IReadOnlyList<PedidoVendaDto>> Handle(GetPVsAprovadosQuery q, CancellationToken ct)
        => coreService.GetPedidosVendaAprovadosAsync(ct);
}
