using AMR.WMS.Infrastructure;
using AMR.WMS.Infrastructure.Data;
using AMR.WMS.API.Middleware;
using AMR.WMS.Application.Behaviors;
using AMR.WMS.Application.Localizacoes.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AMR.WMS.API")
    .WriteTo.Console(
        outputTemplate: ctx.HostingEnvironment.IsProduction()
            ? "[{Timestamp:o} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
            : "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AMR.WMS API", Version = "v1" });
    c.EnableAnnotations();
});

// ── MediatR + ValidationBehavior ─────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CriarLocalizacaoCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// ── FluentValidation ─────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(CriarLocalizacaoCommand).Assembly);

// ── Infrastructure ────────────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Rate Limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window      = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                QueueLimit  = 0,
            }));
    options.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.Headers.RetryAfter = "60";
        await ctx.HttpContext.Response.WriteAsync("Too many requests. Retry after 60 seconds.", ct);
    };
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("AmrWms", policy =>
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "*")
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

// Auto-migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AmrWmsDbContext>();
    db.Database.Migrate();
    await AmrWmsSeed.AplicarAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();

app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.000}ms)";
});

// ── Security Headers ──────────────────────────────────────────────────────────
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"]        = "DENY";
    ctx.Response.Headers["X-XSS-Protection"]       = "1; mode=block";
    ctx.Response.Headers["Referrer-Policy"]        = "strict-origin-when-cross-origin";
    ctx.Response.Headers["Permissions-Policy"]     = "geolocation=(), microphone=(), camera=()";
    if (!ctx.Request.IsHttps && app.Environment.IsProduction())
        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    await next();
});

app.UseCors("AmrWms");
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();
