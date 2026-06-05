using AMR.WMS.Application.Separacao.Commands;
using AMR.WMS.Application.Separacao.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AMR.WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeparacaoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista todas as ordens de separação")]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarOrdensSeparacaoQuery(), ct);
        return Ok(result);
    }

    [HttpGet("pv-aprovados")]
    [SwaggerOperation(Summary = "Lista pedidos de venda aprovados no AMR-Core")]
    public async Task<IActionResult> PVsAprovados(CancellationToken ct)
    {
        var result = await mediator.Send(new GetPVsAprovadosQuery(), ct);
        return Ok(result);
    }

    [HttpPost("criar")]
    [SwaggerOperation(Summary = "Cria uma nova ordem de separação")]
    public async Task<IActionResult> Criar([FromBody] CriarOrdemSeparacaoCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }

    [HttpPut("{id:guid}/separar-item")]
    [SwaggerOperation(Summary = "Registra a separação de um item")]
    public async Task<IActionResult> SepararItem(Guid id, [FromBody] SepararItemRequest req, CancellationToken ct)
    {
        await mediator.Send(new SepararItemCommand(id, req.ItemSeparacaoId, req.LocalizacaoId, req.Quantidade), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/concluir")]
    [SwaggerOperation(Summary = "Conclui a ordem de separação")]
    public async Task<IActionResult> Concluir(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ConcluirSeparacaoCommand(id), ct);
        return NoContent();
    }
}

public record SepararItemRequest(Guid ItemSeparacaoId, Guid LocalizacaoId, int Quantidade);
