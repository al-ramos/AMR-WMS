namespace AMR.WMS.Application.Dashboard;

public record DashboardWmsKpisDto(
    int    TotalLocalizacoes,
    double OcupacaoMediaPct,
    int    LocalizacoesAlerta,
    int    OrdensSeparacao,
    int    OrdensRecebimento
);

public record OcupacaoPorZonaDto(
    string Zona,
    int    TotalLocalizacoes,
    int    CapacidadeTotal,
    int    OcupacaoTotal,
    double OcupacaoPct
);

public record TopProdutoDto(
    int ProdutoId,
    int TotalMovimentacoes,
    int TotalQuantidade
);

public record AlertaLocalizacaoDto(
    Guid   Id,
    string Codigo,
    string Zona,
    int    Ocupacao,
    int    Capacidade,
    double OcupacaoPct
);

public interface IDashboardRepository
{
    Task<DashboardWmsKpisDto>              GetKpisAsync(int dias, CancellationToken ct = default);
    Task<IReadOnlyList<OcupacaoPorZonaDto>> GetOcupacaoPorZonaAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TopProdutoDto>>     GetTopProdutosAsync(int dias, CancellationToken ct = default);
    Task<IReadOnlyList<AlertaLocalizacaoDto>> GetAlertasAsync(CancellationToken ct = default);
}
