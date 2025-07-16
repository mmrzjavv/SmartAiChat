using Carter;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartAiChat.API.Extensions;
using SmartAiChat.API.Hubs;
using SmartAiChat.API.Middleware;
using SmartAiChat.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// Add custom service configurations
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddApplicationHealthChecks(builder.Configuration);
builder.Services.AddRateLimiting();

// Add Carter
builder.Services.AddCarter();

// Configure Swagger specifically for Carter/Minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartAiChat API",
        Version = "v1",
        Description = "SmartAiChat API for AI-powered chat functionality"
    });

    // Essential for Carter - resolve conflicting actions
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Carter-specific operation ID generation
    c.CustomOperationIds(apiDesc =>
    {
        // For Carter endpoints, use the endpoint name if available
        var endpointName = apiDesc.ActionDescriptor.EndpointMetadata?
            .OfType<Microsoft.AspNetCore.Routing.EndpointNameMetadata>()
            .FirstOrDefault()?.EndpointName;

        if (!string.IsNullOrEmpty(endpointName))
        {
            return endpointName;
        }

        // Fallback: create unique ID from route and method
        var routeTemplate = apiDesc.RelativePath?.Replace("/", "_").Replace("{", "").Replace("}", "");
        var httpMethod = apiDesc.HttpMethod?.ToLower();
        
        return $"{httpMethod}_{routeTemplate}_{apiDesc.GetHashCode()}";
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartAiChat API v1");
        c.RoutePrefix = "swagger";
    });
}

// Middleware order is important
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<SecureHeadersMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Map Carter modules
app.MapCarter();

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

// Map Health Checks
app.MapHealthChecks("/health");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
        Log.Information("Database ensured created successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while ensuring database creation");
        throw;
    }
}

Log.Information("SmartAiChat API starting up...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
