using AMR.WMS.Domain.Enums;
using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AMR.WMS.Application.Localizacoes.Commands;

public record CriarLocalizacaoCommand(
    string Zona,
    string Corredor,
    string Prateleira,
    string Posicao,
    int Capacidade,
    TipoLocalizacao TipoLocalizacao = TipoLocalizacao.Picking
) : IRequest<Guid>;

public class CriarLocalizacaoCommandValidator : AbstractValidator<CriarLocalizacaoCommand>
{
    public CriarLocalizacaoCommandValidator()
    {
        RuleFor(x => x.Zona).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Corredor).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Prateleira).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Posicao).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Capacidade).GreaterThan(0);
    }
}

public class CriarLocalizacaoCommandHandler(
    ILocalizacaoRepository repo,
    IUnitOfWork uow) : IRequestHandler<CriarLocalizacaoCommand, Guid>
{
    public async Task<Guid> Handle(CriarLocalizacaoCommand cmd, CancellationToken ct)
    {
        var codigo = $"{cmd.Zona.ToUpper()}-{cmd.Corredor.ToUpper()}-{cmd.Prateleira.ToUpper()}-{cmd.Posicao.ToUpper()}";
        var existente = await repo.ObterPorCodigoAsync(codigo, ct);
        if (existente is not null)
            throw new InvalidOperationException($"Localização com código '{codigo}' já existe.");

        var localizacao = Localizacao.Criar(cmd.Zona, cmd.Corredor, cmd.Prateleira, cmd.Posicao, cmd.Capacidade, cmd.TipoLocalizacao);
        await repo.AdicionarAsync(localizacao, ct);
        await uow.SaveChangesAsync(ct);
        return localizacao.Id;
    }
}
