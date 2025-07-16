using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.Queries.Tenants;
using SmartAiChat.Application.Queries.Users;
using SmartAiChat.Shared.Constants;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.API.Modules;

public class AdminModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/admin")
            .WithTags("Admin")
            .WithOpenApi()
            .RequireAuthorization(SystemConstants.Policies.TenantAdminOrAbove);

        // User management endpoints
        group.MapGet("/users", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves all users for the current tenant");

        group.MapGet("/users/{id:guid}", GetUser)
            .WithName("GetUser")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a specific user");

        group.MapPost("/users", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user for the current tenant");

        group.MapPut("/users/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update a user")
            .WithDescription("Updates an existing user");

        group.MapDelete("/users/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .WithDescription("Deletes a user");

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

        group.MapPost("/tenants", CreateTenant)
            .WithName("CreateTenant")
            .WithSummary("Create a new tenant")
            .WithDescription("Creates a new tenant (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly);


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

    private static async Task<IResult> GetTenants([AsParameters] PaginationRequest request, ISender sender)
    {
        var query = new GetAllTenantsQuery { Pagination = request };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTenant(Guid id, ISender sender)
    {
        var query = new GetTenantByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateTenant(CreateTenantCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return Results.Created($"/api/v1/admin/tenants/{result.Id}", result);
    }

    private static async Task<IResult> UpdateTenant(Guid id, UpdateTenantCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("ID mismatch");
        }
        var result = await sender.Send(command);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteTenant(Guid id, ISender sender)
    {
        var command = new DeleteTenantCommand { Id = id };
        await sender.Send(command);
        return Results.NoContent();
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

    private static async Task<IResult> GetUsers([AsParameters] PaginationRequest request, ISender sender)
    {
        var query = new GetAllUsersQuery { Pagination = request };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUser(Guid id, ISender sender)
    {
        var query = new GetUserByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateUser(CreateUserCommand command, ISender sender, HttpContext httpContext)
    {
        var tenantId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
        if (tenantId == null)
        {
            return Results.BadRequest("TenantId not found in token.");
        }
        command.TenantId = Guid.Parse(tenantId);
        var result = await sender.Send(command);
        return Results.Created($"/api/v1/admin/users/{result.Id}", result);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("ID mismatch");
        }
        var result = await sender.Send(command);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteUser(Guid id, ISender sender)
    {
        var command = new DeleteUserCommand { Id = id };
        await sender.Send(command);
        return Results.NoContent();
    }
} 