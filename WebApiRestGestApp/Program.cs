using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Repositories;
using WebApiRestGestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Entity Framework con PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
           .UseLowerCaseNamingConvention());

// 2. Inyección de dependencias (IoC)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegistroEventoService, RegistroEventoService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // <-- REGISTRAR EL NUEVO SERVICIO

// 3. Configurar Autenticación con JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "TuClaveSuperSecretaDeDesarrollo123!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // AGREGAR ESTO: Interceptar el error de autorización (401) para retornar tu JSON personalizado
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // Bloqueamos el comportamiento por defecto de .NET
                context.HandleResponse();

                // Establecemos el código de estado y el tipo de contenido
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                // Escribimos tu JSON exacto
                var customResponse = "{\n    \"error\": \"UNAUTHORIZED\"\n}";
                await context.Response.WriteAsync(customResponse);
            }
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 4. Configurar Swagger para soportar el envío del Token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestApp API", Version = "v1" });

    // Definir el esquema de seguridad (Botón "Authorize" en Swagger)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Escribe 'Bearer' [espacio] y luego tu token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UsePathBase("/gestapp");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 5. Agregar el Middleware de Autenticación SIEMPRE antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();