using AMR.WMS.Domain.Enums;

namespace AMR.WMS.Domain.Entities;

public class MovimentacaoEstoque
{
    public Guid             Id            { get; private set; }
    public int              ProdutoId     { get; private set; }
    public Guid             LocalizacaoId { get; private set; }
    public TipoMovimentacao Tipo          { get; private set; }
    public int              Quantidade    { get; private set; }
    public DateTime         DataHora      { get; private set; }
    public string?          Observacao    { get; private set; }

    public Localizacao? Localizacao { get; private set; }

    private MovimentacaoEstoque() { }

    public static MovimentacaoEstoque Registrar(
        int produtoId, Guid localizacaoId, TipoMovimentacao tipo,
        int quantidade, string? observacao = null)
    {
        if (produtoId <= 0) throw new ArgumentException("ProdutoId inválido.", nameof(produtoId));
        if (localizacaoId == Guid.Empty) throw new ArgumentException("LocalizacaoId inválido.", nameof(localizacaoId));
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        return new MovimentacaoEstoque
        {
            Id            = Guid.NewGuid(),
            ProdutoId     = produtoId,
            LocalizacaoId = localizacaoId,
            Tipo          = tipo,
            Quantidade    = quantidade,
            DataHora      = DateTime.UtcNow,
            Observacao    = observacao?.Trim(),
        };
    }
}
