using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Serilog — tüm sink/level yapılandırması appsettings.json üzerinden yönetilir
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

// OpenAPI
builder.Services.AddOpenApi();

// Authentication (Keycloak JWT) — fail-fast doğrulama
var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
var keycloakAudience = builder.Configuration["Keycloak:Audience"];

if (string.IsNullOrWhiteSpace(keycloakAuthority) ||
    keycloakAuthority.Contains("__SET_IN", StringComparison.OrdinalIgnoreCase) ||
    keycloakAuthority.Contains("placeholder", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Keycloak JWT yapılandırması geçersiz: 'Keycloak:Authority' değeri boş veya placeholder. " +
        "Lütfen geçerli bir Authority değeri sağlayın (örn. environment variable veya user-secrets üzerinden).");
}

if (string.IsNullOrWhiteSpace(keycloakAudience) ||
    keycloakAudience.Contains("__SET_IN", StringComparison.OrdinalIgnoreCase) ||
    keycloakAudience.Contains("placeholder", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Keycloak JWT yapılandırması geçersiz: 'Keycloak:Audience' değeri boş veya placeholder. " +
        "Lütfen geçerli bir Audience değeri sağlayın (örn. environment variable veya user-secrets üzerinden).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = keycloakAudience;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Wolverine CQRS
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(QRPayCheck.Application.AssemblyMarker).Assembly);
});

builder.Services.AddWolverineHttp();

var app = builder.Build();

// Middleware
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

// OpenAPI + Scalar (yalnızca geliştirme ortamı)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "QR Pay Check API";
    });
}

app.MapWolverineEndpoints();

app.Run();

public partial class Program { }
