using Microsoft.EntityFrameworkCore;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Repositories;
using WebApiRestGestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con PostgreSQL y forzar nombres en minúsculas
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
           .UseLowerCaseNamingConvention()); 

// Inyección de dependencias (IoC)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Dinámico para todas las tablas
builder.Services.AddScoped<IUserService, UserService>(); // Específico para lógica de Usuarios

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar la ruta base para que coincida con la configuración de Apache
app.UsePathBase("/gestapp");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Habilitar Swagger en producción para probar
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();