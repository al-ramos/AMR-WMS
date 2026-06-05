using AMR.WMS.Domain.Enums;
using AMR.WMS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Commands;

public record AtualizarLocalizacaoCommand(
    Guid Id,
    int Capacidade,
    TipoLocalizacao TipoLocalizacao
) : IRequest<Unit>;

public class AtualizarLocalizacaoCommandValidator : AbstractValidator<AtualizarLocalizacaoCommand>
{
    public AtualizarLocalizacaoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Capacidade).GreaterThan(0);
    }
}

public class AtualizarLocalizacaoCommandHandler(
    ILocalizacaoRepository repo,
    IUnitOfWork uow) : IRequestHandler<AtualizarLocalizacaoCommand, Unit>
{
    public async Task<Unit> Handle(AtualizarLocalizacaoCommand cmd, CancellationToken ct)
    {
        var loc = await repo.ObterPorIdAsync(cmd.Id, ct)
            ?? throw new InvalidOperationException($"Localização '{cmd.Id}' não encontrada.");

        loc.Atualizar(cmd.Capacidade, cmd.TipoLocalizacao);
        repo.Atualizar(loc);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
