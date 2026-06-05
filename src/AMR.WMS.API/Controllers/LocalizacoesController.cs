using AMR.WMS.Application.Localizacoes.Commands;
using AMR.WMS.Application.Localizacoes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AMR.WMS.API.Controllers;

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

    [HttpPost]
    [SwaggerOperation(Summary = "Cria uma nova localização")]
    public async Task<IActionResult> Criar([FromBody] CriarLocalizacaoCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }
}
