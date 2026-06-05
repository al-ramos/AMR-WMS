using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Enums;
using AMR.WMS.Domain.Interfaces;
using MediatR;

namespace AMR.WMS.Application.Separacao.Queries;

public record ItemSeparacaoDto(
    Guid  Id,
    int   ProdutoId,
    Guid? LocalizacaoId,
    int   QntSolicitada,
    int   QntSeparada
);

public record OrdemSeparacaoDto(
    Guid                        Id,
    int                         PedidoVendaId,
    StatusSeparacao             Status,
    DateTime                    DataCriacao,
    IReadOnlyList<ItemSeparacaoDto> Itens
);

public record ListarOrdensSeparacaoQuery : IRequest<IReadOnlyList<OrdemSeparacaoDto>>;

public class ListarOrdensSeparacaoQueryHandler(IOrdemSeparacaoRepository repo)
    : IRequestHandler<ListarOrdensSeparacaoQuery, IReadOnlyList<OrdemSeparacaoDto>>
{
    public async Task<IReadOnlyList<OrdemSeparacaoDto>> Handle(ListarOrdensSeparacaoQuery q, CancellationToken ct)
    {
        var lista = await repo.ListarAsync(ct);
        return lista.Select(ToDto).ToList();
    }

    private static OrdemSeparacaoDto ToDto(OrdemSeparacao o) => new(
        o.Id,
        o.PedidoVendaId,
        o.Status,
        o.DataCriacao,
        o.Itens.Select(i => new ItemSeparacaoDto(
            i.Id, i.ProdutoId, i.LocalizacaoId, i.QntSolicitada, i.QntSeparada)).ToList());
}
