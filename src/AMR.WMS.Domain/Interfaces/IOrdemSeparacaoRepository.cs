using AMR.WMS.Domain.Entities;

namespace AMR.WMS.Domain.Interfaces;

public interface IOrdemSeparacaoRepository
{
    Task<OrdemSeparacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrdemSeparacao>> ListarAsync(CancellationToken ct = default);
    Task AdicionarAsync(OrdemSeparacao ordem, CancellationToken ct = default);
    void Atualizar(OrdemSeparacao ordem);
}
