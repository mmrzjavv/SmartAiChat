using Carter;
using Serilog;
using SmartAiChat.API.Extensions;
using SmartAiChat.API.Hubs;
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
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsConfiguration();
builder.Services.AddApplicationHealthChecks(builder.Configuration);

// Add Carter
builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartAiChat API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

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
    }
}

Log.Information("SmartAiChat API starting up...");

app.Run();
