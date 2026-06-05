using AMR.WMS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AMR.WMS.Application.Separacao.Commands;

public record ConcluirSeparacaoCommand(Guid OrdemSeparacaoId) : IRequest<Unit>;

public class ConcluirSeparacaoCommandValidator : AbstractValidator<ConcluirSeparacaoCommand>
{
    public ConcluirSeparacaoCommandValidator()
    {
        RuleFor(x => x.OrdemSeparacaoId).NotEmpty();
    }
}

public class ConcluirSeparacaoCommandHandler(
    IOrdemSeparacaoRepository repo,
    IUnitOfWork uow) : IRequestHandler<ConcluirSeparacaoCommand, Unit>
{
    public async Task<Unit> Handle(ConcluirSeparacaoCommand cmd, CancellationToken ct)
    {
        var ordem = await repo.ObterPorIdAsync(cmd.OrdemSeparacaoId, ct)
            ?? throw new InvalidOperationException($"Ordem '{cmd.OrdemSeparacaoId}' não encontrada.");

        ordem.Concluir();
        repo.Atualizar(ordem);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
