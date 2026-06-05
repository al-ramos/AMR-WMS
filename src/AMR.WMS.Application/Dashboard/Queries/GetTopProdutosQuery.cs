using AMR.WMS.Application.Dashboard;
using MediatR;

namespace AMR.WMS.Application.Dashboard.Queries;

public record GetTopProdutosQuery(int Dias = 30) : IRequest<IReadOnlyList<TopProdutoDto>>;

public class GetTopProdutosQueryHandler(IDashboardRepository repo)
    : IRequestHandler<GetTopProdutosQuery, IReadOnlyList<TopProdutoDto>>
{
    public Task<IReadOnlyList<TopProdutoDto>> Handle(GetTopProdutosQuery q, CancellationToken ct)
        => repo.GetTopProdutosAsync(q.Dias, ct);
}
