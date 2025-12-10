using Datadog.Trace;
using Datadog.Trace.Configuration;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Repository;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Service;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Service;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Infraestrutura.Repositories;
using FIAP.MicroService.Jogos.Infraestrutura.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using Serilog;
using Serilog.Events;
using System.Text.Json;

var settings = TracerSettings.FromDefaultSources();
Tracer.Configure(settings);

var defaultLogger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
   .Enrich.FromLogContext()
   .Enrich.WithEnvironmentName()
   .Enrich.WithMachineName()
   .Enrich.WithProcessId()
   .Enrich.WithThreadId()
   .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter(renderMessage: true))
   .CreateLogger();

Log.Logger = defaultLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();

#region DbContext

// ------------------------------
// Configuração do DbContext
// ------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("A string de conexão não foi encontrada!");

builder.Services.AddDbContext<JogosDbContext>(options => options.UseSqlServer(connectionString));

#endregion

builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IJogoRepository, JogoRepository>();

#region OpenSearch

builder.Services.AddOptions<OpenSearchSettings>()
    .BindConfiguration(nameof(OpenSearchSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IOpenSearchClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<OpenSearchSettings>>().Value;

    var connectionStrings = new ConnectionSettings(new Uri(settings.Endpoint))
        .BasicAuthentication(settings.Username, settings.Password)
        .DefaultIndex("jogos");

    return new OpenSearchClient(connectionStrings);
});

#endregion

#region FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<Jogo>();

#endregion

#region Health Checks

// ------------------------------
// Configuração de Health Checks para Kubernetes
// ------------------------------

builder.Services.AddHealthChecks()
    // Verifica conexão com SQL Server
    .AddSqlServer(
        connectionString: connectionString,
        name: "sqlserver",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "ready" })
    // Verifica se a aplicação está respondendo (liveness básico)
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), 
        tags: new[] { "live" });

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

#region Health Check Endpoints

// ------------------------------
// Endpoints de Health Check
// ------------------------------

// Endpoint principal - verifica tudo (para readinessProbe do K8s)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = WriteHealthCheckResponse
});

// Endpoint de liveness - verifica apenas se a API está viva (para livenessProbe do K8s)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthCheckResponse
});

// Endpoint de readiness - verifica dependências (para readinessProbe do K8s)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthCheckResponse
});

// Função para formatar resposta JSON dos health checks
static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    var response = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            duration = e.Value.Duration.TotalMilliseconds,
            description = e.Value.Description,
            exception = e.Value.Exception?.Message,
            tags = e.Value.Tags
        })
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
}

#endregion

try
{
    Log.Information("Aplicação está iniciando!!!");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal("Erro crítico no sistema: ", ex);
}
finally
{
    Log.CloseAndFlush();
}