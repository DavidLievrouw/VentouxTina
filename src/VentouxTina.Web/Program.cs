using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using VentouxTina.Web.Api;
using VentouxTina.Web.Components;
using VentouxTina.Web.Domain.Services;
using VentouxTina.Web.Infrastructure.DataSources;

var builder = WebApplication.CreateBuilder(args);

// ── Logging & correlation ────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ── Database ─────────────────────────────────────────────────────────────────
var connectionString =
    builder.Configuration.GetConnectionString("MariaDb")
    ?? throw new InvalidOperationException("ConnectionStrings:MariaDb is not configured.");

// Register DbContext factory instead of scoped DbContext to prevent reuse issues
builder.Services.AddTransient<Func<VentouxTinaDbContext>>(provider =>
{
    return () =>
    {
        var optionsBuilder = new DbContextOptionsBuilder<VentouxTinaDbContext>();
        optionsBuilder
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(11, 4, 0)),
                mySql => mySql.SchemaBehavior(MySqlSchemaBehavior.Ignore)
            )
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

        return new VentouxTinaDbContext(optionsBuilder.Options);
    };
});

// ── Memory cache (1-minute progress projection cache) ────────────────────────
builder.Services.AddMemoryCache();

// ── Domain / infrastructure services ─────────────────────────────────────────
builder.Services.AddScoped<IProgressDataSource, ProgressQueryService>();
builder.Services.AddScoped<IProgressService, CachedProgressService>();
builder.Services.AddScoped<IProjectContextDataSource, ProjectContextQueryService>();
builder.Services.AddScoped<ITripLogDataSource, TripLogQueryService>();

// ── Rate limiting ─────────────────────────────────────────────────────────────
var rateLimitSection = builder.Configuration.GetSection("RateLimit");
var permitLimit = rateLimitSection.GetValue("PermitLimit", 60);
var windowSeconds = rateLimitSection.GetValue("WindowSeconds", 60);
var queueLimit = rateLimitSection.GetValue("QueueLimit", 2);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "fixed",
        opt =>
        {
            opt.PermitLimit = permitLimit;
            opt.Window = TimeSpan.FromSeconds(windowSeconds);
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = queueLimit;
        }
    );
    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                code = "TOO_MANY_REQUESTS",
                message = "Te veel verzoeken. Probeer het later opnieuw.",
            },
            ct
        );
    };
});

// ── Blazor / Razor components ─────────────────────────────────────────────────
builder.Services.AddRazorComponents();

// ── Localization (nl-BE) ──────────────────────────────────────────────────────
builder.Services.AddLocalization();

var app = builder.Build();

// ── Startup: DB provisioning and migrations ───────────────────────────────────
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
await StartupMigrationRunner.RunAsync(app.Services, startupLogger);

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseRateLimiter();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

// ── API endpoints ─────────────────────────────────────────────────────────────
app.MapPublicEndpoints();

app.Run();
