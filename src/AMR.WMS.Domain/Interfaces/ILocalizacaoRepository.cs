using AMR.WMS.Domain.Entities;

namespace AMR.WMS.Domain.Interfaces;

public interface ILocalizacaoRepository
{
    Task<Localizacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Localizacao?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<IReadOnlyList<Localizacao>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Localizacao>> ListarDisponiveisAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Localizacao>> ListarPorZonaAsync(string zona, CancellationToken ct = default);
    Task AdicionarAsync(Localizacao localizacao, CancellationToken ct = default);
    void Atualizar(Localizacao localizacao);
    void Remover(Localizacao localizacao);
}
