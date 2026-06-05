using AMR.WMS.Domain.Interfaces;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Queries;

public record ListarPorZonaQuery(string Zona) : IRequest<IReadOnlyList<LocalizacaoDto>>;

public class ListarPorZonaQueryHandler(ILocalizacaoRepository repo)
    : IRequestHandler<ListarPorZonaQuery, IReadOnlyList<LocalizacaoDto>>
{
    public async Task<IReadOnlyList<LocalizacaoDto>> Handle(ListarPorZonaQuery query, CancellationToken ct)
    {
        var lista = await repo.ListarPorZonaAsync(query.Zona, ct);
        return lista.Select(l => new LocalizacaoDto(
            l.Id, l.Codigo, l.Zona, l.Corredor, l.Prateleira, l.Posicao,
            l.Capacidade, l.Ocupacao, l.TipoLocalizacao, l.CriadoEm)).ToList();
    }
}
