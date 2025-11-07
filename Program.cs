using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Implementations;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;
using complejoDeportivo.Repositories;
using complejoDeportivo.Services;
using complejoDeportivo.Repositories.Dashboard;
using Microsoft.OpenApi.Models; // <-- AÑADIDO ESTE USING

var builder = WebApplication.CreateBuilder(args);

// --- 1. Política de CORS ---
var corsPolicyName = "TPIPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:5500")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// --- 2. Conexión a la Base de Datos ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ComplejoDeportivoContext>(options =>
    options.UseSqlServer(connectionString));

// --- 3. Configuración de Autenticación JWT ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
#pragma warning disable CS8604 // Possible null reference argument.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };
#pragma warning restore CS8604 // Possible null reference argument.
    });
builder.Services.AddAuthorization();

// --- 4. Inyección de Dependencias (Registrar TODO) ---

// Auth
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Usuario
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Cliente
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// Empleado
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

// Complejo
builder.Services.AddScoped<IComplejoRepository, ComplejoRepository>();
builder.Services.AddScoped<IComplejoService, ComplejoService>();

// Cancha y TipoCancha
builder.Services.AddScoped<ITipoCanchaRepository, TipoCanchaRepository>();
builder.Services.AddScoped<ITipoCanchaService, TipoCanchaService>();
builder.Services.AddScoped<ICanchaRepository, CanchaRepository>();
builder.Services.AddScoped<ICanchaService, CanchaService>();

// Reservas
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaServicie, ReservaServicie>();

// Dashboard
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


// --- 5. Servicios de la Plantilla ---
// Añadir soporte para DateOnly y TimeOnly en JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new Support.DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new Support.TimeOnlyJsonConverter());
    });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// --- ESTA ES LA PARTE MODIFICADA ---
// Se reemplaza "builder.Services.AddSwaggerGen();" por este bloque:
builder.Services.AddSwaggerGen(options =>
{
    // Añade un título a tu Swagger
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Complejo Deportivo API", Version = "v1" });

    // Define el esquema de seguridad (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Usamos Http para Bearer
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' [espacio] y luego tu token. \r\n\r\nEjemplo: \"Bearer tu_token_aqui\""
    });

    // Aplica este requisito de seguridad a todos los endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
// --- FIN DE LA MODIFICACIÓN ---


// --- 6. Construir la App ---
var app = builder.Build();

// --- 7. Configurar el Pipeline de HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


// --- Clases de Soporte para DateOnly/TimeOnly en .NET 8 ---
namespace Support
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }

    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string Format = "HH:mm:ss";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}