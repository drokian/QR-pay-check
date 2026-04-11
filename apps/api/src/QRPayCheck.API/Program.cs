using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    var fileEnabled = ctx.Configuration.GetValue<bool?>("Logging:File:Enabled")
        ?? ctx.HostingEnvironment.IsDevelopment();

    if (fileEnabled)
    {
        var configuredPath = ctx.Configuration["Logging:File:Path"];
        var logPath = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(AppContext.BaseDirectory, "logs", "qrpaycheck-.log")
            : Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(AppContext.BaseDirectory, configuredPath);

        lc.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
    }
});

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
