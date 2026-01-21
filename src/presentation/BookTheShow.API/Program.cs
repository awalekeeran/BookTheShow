using BookTheShow.API;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;

// 🔹 Configure Serilog EARLY (before building the host)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger(); // Bootstrap logger for startup errors

try
{
    Log.Information("🚀 Starting BookTheShow API...");

    var builder = WebApplication.CreateBuilder(args);

    var diagnosticsSettings = builder.Configuration
        .GetSection(DiagnosticsSettings.SectionName)
        .Get<DiagnosticsSettings>() ?? new DiagnosticsSettings();

    builder.Services.Configure<DiagnosticsSettings>(
        builder.Configuration.GetSection(DiagnosticsSettings.SectionName));

    // 🔹 Replace default logging with Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProcessId()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", "BookTheShow.API")
    );

    // Add services to the container.

    // 1️⃣ Swagger/OpenAPI Configuration
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookTheShow API",
        Version = "v1",
        Description = "Ticket booking platform API - Ticketmaster style system for learning system design & patterns",
        Contact = new OpenApiContact
        {
            Name = "BookTheShow Team",
            Url = new Uri("https://github.com/awalekeeran/BookTheShow")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // JWT Authentication support in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Enable XML comments for better Swagger documentation (optional)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // options.IncludeXmlComments(xmlPath);
});

    // 2️⃣ CORS - Allow React frontend to call API
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:65044", "https://localhost:65044") // React dev server
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    // 🔹 Serilog Request Logging (MUST be first middleware)
    if (diagnosticsSettings.EnableRequestLogging)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            // Customize log level based on status code, elapsed time, and exceptions
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;
                if (elapsed > 1000) return LogEventLevel.Warning;
                return LogEventLevel.Information;
            };

            // Filter out noise (static files, swagger, health checks)
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                var path = httpContext.Request.Path.Value ?? string.Empty;

                // Skip static files unless explicitly enabled
                if (!diagnosticsSettings.LogStaticFiles && IsStaticFile(path))
                    return LogEventLevel.Verbose;

                // Skip health checks unless explicitly enabled
                if (!diagnosticsSettings.LogHealthChecks && (path.StartsWith("/health") || path.StartsWith("/ready")))
                    return LogEventLevel.Verbose;

                // Skip swagger assets unless detailed diagnostics enabled
                if (!diagnosticsSettings.EnableDetailedDiagnostics && IsSwaggerAsset(path))
                    return LogEventLevel.Verbose;

                // Normal request handling
                if (ex != null) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;
                if (elapsed > 1000) return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };

            // Add diagnostic context only when detailed diagnostics is enabled
            if (diagnosticsSettings.EnableDetailedDiagnostics)
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                    diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("Protocol", httpContext.Request.Protocol);
                };
            }
        });
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookTheShow API v1");
            options.RoutePrefix = string.Empty; // Swagger at root: http://localhost:5000
        });
    }

    app.UseHttpsRedirection();

    // Sample endpoint
    var summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    app.MapGet("/weatherforecast", () =>
    {
        Log.Information("Weather forecast requested");
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

    Log.Information("✅ BookTheShow API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
}
finally
{
    Log.Information("🛑 Shutting down BookTheShow API");
    await Log.CloseAndFlushAsync();
}

// Helper methods for filtering
static bool IsStaticFile(string path) =>
    path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".map") ||
    path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".ico") ||
    path.EndsWith(".svg") || path.EndsWith(".woff") || path.EndsWith(".woff2") ||
    path.EndsWith(".ttf") || path.EndsWith(".eot");

static bool IsSwaggerAsset(string path) =>
    path.Contains("/swagger") || path.Contains("swagger.json") ||
    path.Contains("swagger-ui") || path.Contains("_framework");


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
