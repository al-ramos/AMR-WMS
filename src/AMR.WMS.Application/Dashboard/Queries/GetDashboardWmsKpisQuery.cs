using AMR.WMS.Application.Dashboard;
using MediatR;

namespace AMR.WMS.Application.Dashboard.Queries;

public record GetDashboardWmsKpisQuery(int Dias = 1) : IRequest<DashboardWmsKpisDto>;

public class GetDashboardWmsKpisQueryHandler(IDashboardRepository repo)
    : IRequestHandler<GetDashboardWmsKpisQuery, DashboardWmsKpisDto>
{
    public Task<DashboardWmsKpisDto> Handle(GetDashboardWmsKpisQuery q, CancellationToken ct)
        => repo.GetKpisAsync(q.Dias, ct);
}
