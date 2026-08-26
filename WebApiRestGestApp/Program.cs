using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Collections.Generic;
using System.Linq; // Necesario para leer las cabeceras
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Repositories;
using WebApiRestGestApp.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Entity Framework con PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
           .UseLowerCaseNamingConvention());

// 2. Inyección de dependencias (IoC)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegistroEventoService, RegistroEventoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEtapaGestacionalService, EtapaGestacionalService>();
builder.Services.AddScoped<IRegistroSuplementoService, RegistroSuplementoService>();
builder.Services.AddScoped<IRegistroHemoglobinaService, RegistroHemoglobinaService>();
builder.Services.AddScoped<IDiasGestacionService, DiasGestacionService>();
builder.Services.AddScoped<IAgendaGestacionalService, AgendaGestacionalService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
// Registrar HttpClient y el nuevo servicio de correo electrónico
builder.Services.AddHttpClient<IEmailService, EmailService>();

// NUEVO: Configurar CORS para permitir que cualquier aplicación o frontend se conecte sin bloqueos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var customResponse = "{\n    \"error\": \"UNAUTHORIZED\"\n}";
                await context.Response.WriteAsync(customResponse);
            }
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 4. Configurar Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestApp API", Version = "v1" });

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

// Agregamos la ruta base para tu servidor Apache
app.UsePathBase("/gestapp");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            // NUEVO: Detectar inteligentemente si Apache envió la petición en HTTPS
            var scheme = httpReq.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? httpReq.Scheme;

            var serverUrl = $"{scheme}://{httpReq.Host.Value}/gestapp";
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = serverUrl }
            };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/gestapp/swagger/v1/swagger.json", "GestApp API v1");
    });
}

// NUEVO: Activar la política de CORS ANTES de la autenticación
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// 5. Agregar el Middleware de Autenticación
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();