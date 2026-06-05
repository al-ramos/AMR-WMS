using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Enums;
using AMR.WMS.Domain.Interfaces;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Queries;

public record LocalizacaoDto(
    Guid Id,
    string Codigo,
    string Zona,
    string Corredor,
    string Prateleira,
    string Posicao,
    int Capacidade,
    int Ocupacao,
    TipoLocalizacao TipoLocalizacao,
    DateTime CriadoEm
);

public record ListarLocalizacoesQuery : IRequest<IReadOnlyList<LocalizacaoDto>>;

public class ListarLocalizacoesQueryHandler(ILocalizacaoRepository repo)
    : IRequestHandler<ListarLocalizacoesQuery, IReadOnlyList<LocalizacaoDto>>
{
    public async Task<IReadOnlyList<LocalizacaoDto>> Handle(ListarLocalizacoesQuery query, CancellationToken ct)
    {
        var lista = await repo.ListarAsync(ct);
        return lista.Select(ToDto).ToList();
    }

    private static LocalizacaoDto ToDto(Localizacao l) => new(
        l.Id, l.Codigo, l.Zona, l.Corredor, l.Prateleira, l.Posicao,
        l.Capacidade, l.Ocupacao, l.TipoLocalizacao, l.CriadoEm);
}
