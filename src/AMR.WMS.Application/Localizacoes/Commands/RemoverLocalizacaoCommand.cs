using AMR.WMS.Domain.Interfaces;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Commands;

public record RemoverLocalizacaoCommand(Guid Id) : IRequest<Unit>;

public class RemoverLocalizacaoCommandHandler(
    ILocalizacaoRepository repo,
    IUnitOfWork uow) : IRequestHandler<RemoverLocalizacaoCommand, Unit>
{
    public async Task<Unit> Handle(RemoverLocalizacaoCommand cmd, CancellationToken ct)
    {
        var loc = await repo.ObterPorIdAsync(cmd.Id, ct)
            ?? throw new InvalidOperationException($"Localização '{cmd.Id}' não encontrada.");

        if (loc.Ocupacao > 0)
            throw new InvalidOperationException("Não é possível remover uma localização com estoque.");

        repo.Remover(loc);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
