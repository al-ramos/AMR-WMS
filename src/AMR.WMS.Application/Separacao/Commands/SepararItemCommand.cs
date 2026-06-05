using AMR.WMS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AMR.WMS.Application.Separacao.Commands;

public record SepararItemCommand(
    Guid OrdemSeparacaoId,
    Guid ItemSeparacaoId,
    Guid LocalizacaoId,
    int  Quantidade
) : IRequest<Unit>;

public class SepararItemCommandValidator : AbstractValidator<SepararItemCommand>
{
    public SepararItemCommandValidator()
    {
        RuleFor(x => x.OrdemSeparacaoId).NotEmpty();
        RuleFor(x => x.ItemSeparacaoId).NotEmpty();
        RuleFor(x => x.LocalizacaoId).NotEmpty();
        RuleFor(x => x.Quantidade).GreaterThan(0);
    }
}

public class SepararItemCommandHandler(
    IOrdemSeparacaoRepository repo,
    ILocalizacaoRepository locRepo,
    IUnitOfWork uow) : IRequestHandler<SepararItemCommand, Unit>
{
    public async Task<Unit> Handle(SepararItemCommand cmd, CancellationToken ct)
    {
        var ordem = await repo.ObterPorIdAsync(cmd.OrdemSeparacaoId, ct)
            ?? throw new InvalidOperationException($"Ordem '{cmd.OrdemSeparacaoId}' não encontrada.");

        var item = ordem.Itens.FirstOrDefault(i => i.Id == cmd.ItemSeparacaoId)
            ?? throw new InvalidOperationException($"Item '{cmd.ItemSeparacaoId}' não encontrado na ordem.");

        var loc = await locRepo.ObterPorIdAsync(cmd.LocalizacaoId, ct)
            ?? throw new InvalidOperationException($"Localização '{cmd.LocalizacaoId}' não encontrada.");

        if (ordem.Status == Domain.Enums.StatusSeparacao.Aberta)
            ordem.IniciarSeparacao();

        item.Separar(cmd.LocalizacaoId, cmd.Quantidade);
        loc.AtualizarOcupacao(-cmd.Quantidade);

        repo.Atualizar(ordem);
        locRepo.Atualizar(loc);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
