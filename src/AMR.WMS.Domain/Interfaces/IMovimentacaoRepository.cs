using AMR.WMS.Domain.Entities;

namespace AMR.WMS.Domain.Interfaces;

public interface IMovimentacaoRepository
{
    Task<IReadOnlyList<MovimentacaoEstoque>> ListarPorLocalizacaoAsync(Guid localizacaoId, CancellationToken ct = default);
    Task AdicionarAsync(MovimentacaoEstoque movimentacao, CancellationToken ct = default);
}
