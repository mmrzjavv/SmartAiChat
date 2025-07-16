using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Shared.Constants;

namespace SmartAiChat.API.Modules;

public class AdminModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/admin")
            .WithTags("Admin")
            .WithOpenApi()
            .RequireAuthorization(SystemConstants.Policies.TenantAdminOrAbove);

        // Tenant management endpoints
        group.MapGet("/tenants", GetTenants)
            .WithName("GetTenants")
            .WithSummary("Get all tenants")
            .WithDescription("Retrieves all tenants (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly);

        group.MapGet("/tenants/{id:guid}", GetTenant)
            .WithName("GetTenant")
            .WithSummary("Get tenant by ID")
            .WithDescription("Retrieves a specific tenant");

        // AI Configuration endpoints
        group.MapGet("/ai-configuration", GetAiConfiguration)
            .WithName("GetAiConfiguration")
            .WithSummary("Get AI configuration")
            .WithDescription("Retrieves the AI configuration for the current tenant");

        // FAQ management endpoints
        group.MapGet("/faqs", GetFaqs)
            .WithName("GetFaqs")
            .WithSummary("Get FAQ entries")
            .WithDescription("Retrieves FAQ entries for the current tenant");

        // Training files endpoints
        group.MapGet("/training-files", GetTrainingFiles)
            .WithName("GetTrainingFiles")
            .WithSummary("Get training files")
            .WithDescription("Retrieves AI training files for the current tenant");
    }

    private static IResult GetTenants()
    {
        // Placeholder - would implement with proper handler
        return Results.Ok(new { message = "Get tenants endpoint - to be implemented" });
    }

    private static IResult GetTenant(Guid id)
    {
        // Placeholder - would implement with proper handler
        return Results.Ok(new { message = $"Get tenant {id} endpoint - to be implemented" });
    }

    private static IResult GetAiConfiguration()
    {
        // Placeholder - would implement with proper handler
        return Results.Ok(new { message = "Get AI configuration endpoint - to be implemented" });
    }

    private static IResult GetFaqs()
    {
        // Placeholder - would implement with proper handler
        return Results.Ok(new { message = "Get FAQs endpoint - to be implemented" });
    }

    private static IResult GetTrainingFiles()
    {
        // Placeholder - would implement with proper handler
        return Results.Ok(new { message = "Get training files endpoint - to be implemented" });
    }
} 