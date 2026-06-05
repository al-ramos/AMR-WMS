using AMR.WMS.Domain.Enums;

namespace AMR.WMS.Domain.Entities;

public class ItemSeparacao
{
    public Guid  Id                { get; private set; }
    public Guid  OrdemSeparacaoId  { get; private set; }
    public int   ProdutoId         { get; private set; }
    public Guid? LocalizacaoId     { get; private set; }
    public int   QntSolicitada     { get; private set; }
    public int   QntSeparada       { get; private set; }

    public Localizacao? Localizacao { get; private set; }

    private ItemSeparacao() { }

    public static ItemSeparacao Criar(Guid ordemId, int produtoId, int qntSolicitada)
    {
        if (produtoId <= 0)    throw new ArgumentException("ProdutoId inválido.", nameof(produtoId));
        if (qntSolicitada <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(qntSolicitada));

        return new ItemSeparacao
        {
            Id               = Guid.NewGuid(),
            OrdemSeparacaoId = ordemId,
            ProdutoId        = produtoId,
            QntSolicitada    = qntSolicitada,
            QntSeparada      = 0,
        };
    }

    public void Separar(Guid localizacaoId, int quantidade)
    {
        if (localizacaoId == Guid.Empty) throw new ArgumentException("LocalizacaoId inválido.", nameof(localizacaoId));
        if (quantidade <= 0)             throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (quantidade > QntSolicitada)  throw new InvalidOperationException("Quantidade separada não pode exceder a solicitada.");

        LocalizacaoId = localizacaoId;
        QntSeparada   = quantidade;
    }
}
