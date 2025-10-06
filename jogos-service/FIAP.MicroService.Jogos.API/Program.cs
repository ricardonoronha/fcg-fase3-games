using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Infraestrutura.Repositories;
using FIAP.MicroService.Jogos.Infraestrutura.Service;
using FIAP.MicroService.Jogos.Infraestrutura.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenSearch.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada no appsettings.json ou está vazia.");
}

builder.Services.AddDbContext<JogosDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IJogoRepository, JogoRepository>();

builder.Services.AddScoped<IJogoService, JogoService>();

builder
    .Services
    .AddOptions<OpenSearchSettings>()
    .BindConfiguration(nameof(OpenSearchSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<OpenSearchSettings>>().Value;

    var openSearchConnectionSettings = new ConnectionSettings(new Uri(settings.Endpoint))
            .BasicAuthentication(settings.Username, settings.Password);

    var client = new OpenSearchClient(openSearchConnectionSettings);

    return client;
});

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

app.Run();