using AMR.WMS.Domain.Enums;

namespace AMR.WMS.Domain.Entities;

public class Localizacao
{
    public Guid             Id              { get; private set; }
    public string           Codigo          { get; private set; } = default!;
    public string           Zona            { get; private set; } = default!;
    public string           Corredor        { get; private set; } = default!;
    public string           Prateleira      { get; private set; } = default!;
    public string           Posicao         { get; private set; } = default!;
    public int              Capacidade      { get; private set; }
    public int              Ocupacao        { get; private set; }
    public TipoLocalizacao  TipoLocalizacao { get; private set; }
    public DateTime         CriadoEm       { get; private set; }
    public DateTime         AlteradoEm     { get; private set; }

    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes => _movimentacoes.AsReadOnly();
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    private Localizacao() { }

    public static Localizacao Criar(
        string zona, string corredor, string prateleira, string posicao,
        int capacidade, TipoLocalizacao tipo = TipoLocalizacao.Picking)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zona);
        ArgumentException.ThrowIfNullOrWhiteSpace(corredor);
        ArgumentException.ThrowIfNullOrWhiteSpace(prateleira);
        ArgumentException.ThrowIfNullOrWhiteSpace(posicao);
        if (capacidade <= 0) throw new ArgumentException("Capacidade deve ser maior que zero.", nameof(capacidade));

        var codigo = $"{zona.ToUpper()}-{corredor.ToUpper()}-{prateleira.ToUpper()}-{posicao.ToUpper()}";

        return new Localizacao
        {
            Id              = Guid.NewGuid(),
            Codigo          = codigo,
            Zona            = zona.ToUpper().Trim(),
            Corredor        = corredor.ToUpper().Trim(),
            Prateleira      = prateleira.ToUpper().Trim(),
            Posicao         = posicao.ToUpper().Trim(),
            Capacidade      = capacidade,
            Ocupacao        = 0,
            TipoLocalizacao = tipo,
            CriadoEm       = DateTime.UtcNow,
            AlteradoEm     = DateTime.UtcNow,
        };
    }

    public void Atualizar(int capacidade, TipoLocalizacao tipo)
    {
        if (capacidade <= 0) throw new ArgumentException("Capacidade deve ser maior que zero.", nameof(capacidade));
        if (capacidade < Ocupacao) throw new InvalidOperationException("Capacidade não pode ser menor que a ocupação atual.");

        Capacidade      = capacidade;
        TipoLocalizacao = tipo;
        AlteradoEm     = DateTime.UtcNow;
    }

    public void AtualizarOcupacao(int delta)
    {
        var novaOcupacao = Ocupacao + delta;
        if (novaOcupacao < 0) throw new InvalidOperationException("Ocupação não pode ser negativa.");
        if (novaOcupacao > Capacidade) throw new InvalidOperationException("Ocupação excede a capacidade da localização.");

        Ocupacao   = novaOcupacao;
        AlteradoEm = DateTime.UtcNow;
    }
}
