using AMR.WMS.Domain.Enums;

namespace AMR.WMS.Domain.Entities;

public class OrdemSeparacao
{
    public Guid             Id             { get; private set; }
    public int              PedidoVendaId  { get; private set; }
    public StatusSeparacao  Status         { get; private set; }
    public DateTime         DataCriacao    { get; private set; }

    public IReadOnlyCollection<ItemSeparacao> Itens => _itens.AsReadOnly();
    private readonly List<ItemSeparacao> _itens = [];

    private OrdemSeparacao() { }

    public static OrdemSeparacao Criar(int pedidoVendaId, IEnumerable<(int ProdutoId, int Quantidade)> itens)
    {
        if (pedidoVendaId <= 0) throw new ArgumentException("PedidoVendaId inválido.", nameof(pedidoVendaId));

        var ordem = new OrdemSeparacao
        {
            Id            = Guid.NewGuid(),
            PedidoVendaId = pedidoVendaId,
            Status        = StatusSeparacao.Aberta,
            DataCriacao   = DateTime.UtcNow,
        };

        foreach (var (produtoId, quantidade) in itens)
            ordem._itens.Add(ItemSeparacao.Criar(ordem.Id, produtoId, quantidade));

        if (ordem._itens.Count == 0)
            throw new InvalidOperationException("A ordem deve ter ao menos um item.");

        return ordem;
    }

    public void IniciarSeparacao()
    {
        if (Status != StatusSeparacao.Aberta)
            throw new InvalidOperationException("Apenas ordens abertas podem ser iniciadas.");
        Status = StatusSeparacao.EmSeparacao;
    }

    public void Concluir()
    {
        if (Status == StatusSeparacao.Concluida)
            throw new InvalidOperationException("Ordem já está concluída.");
        Status = StatusSeparacao.Concluida;
    }
}
