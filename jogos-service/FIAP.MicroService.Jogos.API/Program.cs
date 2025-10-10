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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using Serilog;
using Serilog.Events;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

try
{
    Log.Information("Aplicação está iniciando!");
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