using AMR.WMS.Domain.Interfaces;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Queries;

public record ObterDisponiveisQuery : IRequest<IReadOnlyList<LocalizacaoDto>>;

public class ObterDisponiveisQueryHandler(ILocalizacaoRepository repo)
    : IRequestHandler<ObterDisponiveisQuery, IReadOnlyList<LocalizacaoDto>>
{
    public async Task<IReadOnlyList<LocalizacaoDto>> Handle(ObterDisponiveisQuery query, CancellationToken ct)
    {
        var lista = await repo.ListarDisponiveisAsync(ct);
        return lista.Select(l => new LocalizacaoDto(
            l.Id, l.Codigo, l.Zona, l.Corredor, l.Prateleira, l.Posicao,
            l.Capacidade, l.Ocupacao, l.TipoLocalizacao, l.CriadoEm)).ToList();
    }
}
