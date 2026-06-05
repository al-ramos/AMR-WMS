using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AMR.WMS.Application.Separacao.Commands;

public record ItemSeparacaoInput(int ProdutoId, int Quantidade);

public record CriarOrdemSeparacaoCommand(
    int PedidoVendaId,
    List<ItemSeparacaoInput> Itens
) : IRequest<Guid>;

public class CriarOrdemSeparacaoCommandValidator : AbstractValidator<CriarOrdemSeparacaoCommand>
{
    public CriarOrdemSeparacaoCommandValidator()
    {
        RuleFor(x => x.PedidoVendaId).GreaterThan(0);
        RuleFor(x => x.Itens).NotEmpty();
        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId).GreaterThan(0);
            item.RuleFor(i => i.Quantidade).GreaterThan(0);
        });
    }
}

public class CriarOrdemSeparacaoCommandHandler(
    IOrdemSeparacaoRepository repo,
    IUnitOfWork uow) : IRequestHandler<CriarOrdemSeparacaoCommand, Guid>
{
    public async Task<Guid> Handle(CriarOrdemSeparacaoCommand cmd, CancellationToken ct)
    {
        var ordem = OrdemSeparacao.Criar(
            cmd.PedidoVendaId,
            cmd.Itens.Select(i => (i.ProdutoId, i.Quantidade)));

        await repo.AdicionarAsync(ordem, ct);
        await uow.SaveChangesAsync(ct);
        return ordem.Id;
    }
}
