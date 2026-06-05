using AMR.WMS.Application.Localizacoes.Commands;
using AMR.WMS.Application.Localizacoes.Queries;
using AMR.WMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AMR.WMS.API.Controllers;

public record AtualizarLocalizacaoRequest(int Capacidade, TipoLocalizacao TipoLocalizacao);

[ApiController]
[Route("api/[controller]")]
public class LocalizacoesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista todas as localizações")]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarLocalizacoesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("disponiveis")]
    [SwaggerOperation(Summary = "Lista localizações com espaço disponível")]
    public async Task<IActionResult> Disponiveis(CancellationToken ct)
    {
        var result = await mediator.Send(new ObterDisponiveisQuery(), ct);
        return Ok(result);
    }

    [HttpGet("zona/{zona}")]
    [SwaggerOperation(Summary = "Lista localizações de uma zona específica")]
    public async Task<IActionResult> PorZona(string zona, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPorZonaQuery(zona), ct);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cria uma nova localização")]
    public async Task<IActionResult> Criar([FromBody] CriarLocalizacaoCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Atualiza capacidade e tipo de uma localização")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarLocalizacaoRequest req, CancellationToken ct)
    {
        await mediator.Send(new AtualizarLocalizacaoCommand(id, req.Capacidade, req.TipoLocalizacao), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Remove uma localização sem estoque")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await mediator.Send(new RemoverLocalizacaoCommand(id), ct);
        return NoContent();
    }
}
