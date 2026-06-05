using AMR.WMS.Application.Dashboard;
using MediatR;

namespace AMR.WMS.Application.Dashboard.Queries;

public record GetOcupacaoPorZonaQuery : IRequest<IReadOnlyList<OcupacaoPorZonaDto>>;

public class GetOcupacaoPorZonaQueryHandler(IDashboardRepository repo)
    : IRequestHandler<GetOcupacaoPorZonaQuery, IReadOnlyList<OcupacaoPorZonaDto>>
{
    public Task<IReadOnlyList<OcupacaoPorZonaDto>> Handle(GetOcupacaoPorZonaQuery q, CancellationToken ct)
        => repo.GetOcupacaoPorZonaAsync(ct);
}
