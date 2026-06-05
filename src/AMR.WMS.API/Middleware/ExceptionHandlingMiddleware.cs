using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AMR.WMS.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validação falhou: {Errors}", string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, "Validação falhou.", ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumento inválido");
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Operação inválida");
            await WriteProblem(ctx, StatusCodes.Status422UnprocessableEntity, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado");
            await WriteProblem(ctx, StatusCodes.Status500InternalServerError, "Erro interno do servidor.");
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, string detail,
        Dictionary<string, string[]>? errors = null)
    {
        ctx.Response.StatusCode  = status;
        ctx.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = ReasonPhrase(status),
            Detail = detail,
            Instance = ctx.Request.Path
        };

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOpts));
    }

    private static string ReasonPhrase(int status) => status switch
    {
        400 => "Bad Request",
        422 => "Unprocessable Entity",
        _   => "Internal Server Error"
    };
}
