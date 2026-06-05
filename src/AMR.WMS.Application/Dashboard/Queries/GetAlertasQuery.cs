using AMR.WMS.Application.Dashboard;
using MediatR;

namespace AMR.WMS.Application.Dashboard.Queries;

public record GetAlertasQuery : IRequest<IReadOnlyList<AlertaLocalizacaoDto>>;

public class GetAlertasQueryHandler(IDashboardRepository repo)
    : IRequestHandler<GetAlertasQuery, IReadOnlyList<AlertaLocalizacaoDto>>
{
    public Task<IReadOnlyList<AlertaLocalizacaoDto>> Handle(GetAlertasQuery q, CancellationToken ct)
        => repo.GetAlertasAsync(ct);
}
