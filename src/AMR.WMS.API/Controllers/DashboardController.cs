using AMR.WMS.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AMR.WMS.API.Controllers;

[ApiController]
[Route("api/dashboard/wms")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "KPIs gerais do WMS")]
    public async Task<IActionResult> Kpis([FromQuery] int dias = 1, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetDashboardWmsKpisQuery(dias), ct);
        return Ok(result);
    }

    [HttpGet("ocupacao")]
    [SwaggerOperation(Summary = "Ocupação por zona")]
    public async Task<IActionResult> OcupacaoPorZona(CancellationToken ct)
    {
        var result = await mediator.Send(new GetOcupacaoPorZonaQuery(), ct);
        return Ok(result);
    }

    [HttpGet("top-produtos")]
    [SwaggerOperation(Summary = "Top 10 produtos movimentados")]
    public async Task<IActionResult> TopProdutos([FromQuery] int dias = 30, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTopProdutosQuery(dias), ct);
        return Ok(result);
    }

    [HttpGet("alertas")]
    [SwaggerOperation(Summary = "Localizações com ocupação acima de 90%")]
    public async Task<IActionResult> Alertas(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAlertasQuery(), ct);
        return Ok(result);
    }
}
