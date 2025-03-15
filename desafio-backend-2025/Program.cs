using desafio_backend_2025.Repositories;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Configuração da chave secreta para JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// Configuração do JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; 
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, 
            ValidateAudience = false,
            ValidateLifetime = true, 
            IssuerSigningKey = new SymmetricSecurityKey(key) 
        };
    });


builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); 
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conta Bancária", Version = "v1" });

    // Definição do JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



//coleção de conection de banco de dados
builder.Services.AddSingleton<DatabaseConnection>();

//REGISTRO DE REPOSITORY
builder.Services.AddScoped<ContaRepository>();
builder.Services.AddScoped<TransacaoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddHttpClient<ReceitaWSService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
