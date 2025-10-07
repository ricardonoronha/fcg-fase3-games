using Nest;
using Serilog;
// using OpenSearch.Client;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using FIAP.MicroService.Jogos.Dominio.Service;
using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Infraestrutura.Settings;
using FIAP.MicroService.Jogos.Infraestrutura.Repositories;
using FIAP.MicroService.Jogos.Infraestrutura.ElasticSearch;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Host.UseSerilog();

builder.Services.AddOptions<ElasticsearchSettings>()
                .BindConfiguration(nameof(ElasticsearchSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IJogoElasticRepository, JogoElasticRepository>();

builder.Services.AddSingleton(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ElasticsearchSettings>>().Value;

    var connectionSettings = new ConnectionSettings(new Uri(settings.Uri))
        .BasicAuthentication(settings.Username, settings.Password) 
        .DefaultIndex("jogos"); 

    return new ElasticClient(connectionSettings);
});

builder.Services.AddDbContext<DbContextJogos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddOptions<OpenSearchSettings>()
                .BindConfiguration(nameof(OpenSearchSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

//builder.Services.AddSingleton(serviceProvider =>
//{
//    var settings = serviceProvider.GetRequiredService<IOptions<OpenSearchSettings>>().Value;

//    var openSearchConnectionSettings = new ConnectionSettings(new Uri(settings.Endpoint))
//            .BasicAuthentication(settings.Username, settings.Password);

//    var client = new OpenSearchClient(openSearchConnectionSettings);

//    return client;
//});

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