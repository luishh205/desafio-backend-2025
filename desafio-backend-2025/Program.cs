using desafio_backend_2025.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Habilita anotações do Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conta Bancária", Version = "v1" });
});

//coleção de conection de banco de dados
builder.Services.AddSingleton<DatabaseConnection>();

//REGISTRO DE REPOSITORY
//ContaRepository
builder.Services.AddScoped<ContaRepository>();
//TransacaoRepository
builder.Services.AddScoped<TransacaoRepository>();
//ReceitaWSService
builder.Services.AddHttpClient<ReceitaWSService>();




var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
