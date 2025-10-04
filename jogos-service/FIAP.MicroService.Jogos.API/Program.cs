using FIAP.MicroService.Jogos.Dominio.Interfaces;
using FIAP.MicroService.Jogos.Infraestrutura.Repositories; 
using FIAP.MicroService.Jogos.Infraestrutura.Data;
using FIAP.MicroService.Jogos.Infraestrutura.Service;
using Microsoft.EntityFrameworkCore;

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