using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/qrpaycheck-.log", rollingInterval: RollingInterval.Day));

// OpenAPI
builder.Services.AddOpenApi();

// Authentication (Keycloak JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
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
