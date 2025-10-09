using Serilog;
using FluentValidation;
using OpenSearch.Client;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Service;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Infraestrutura.Settings;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Service;
using FIAP.MicroService.Jogos.Infraestrutura.Repositories;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Repository;

var builder = WebApplication.CreateBuilder(args);

#region Serilog

// ------------------------------
// Configurar Serilog
// ------------------------------

var logPath = Path.Combine(AppContext.BaseDirectory, "Logs");

if (!Directory.Exists(logPath))
    Directory.CreateDirectory(logPath);

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            Path.Combine(logPath, "log-.txt"),
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
            retainedFileCountLimit: 30
        )
        .CreateLogger();

builder.Host.UseSerilog();

#endregion

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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