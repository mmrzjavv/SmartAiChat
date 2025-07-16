using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.AIConfiguration;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.Queries.AIConfiguration;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Application.Queries.Tenants;
using SmartAiChat.Application.Queries.TrainingFile;
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
            .WithDescription("Retrieves all users for the current tenant")
            .Produces<PaginatedResult<UserDto>>(StatusCodes.Status200OK);

        group.MapGet("/users/{id:guid}", GetUser)
            .WithName("GetUser")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a specific user")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/users", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user for the current tenant")
            .Produces<UserDto>(StatusCodes.Status201Created);

        group.MapPut("/users/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update a user")
            .WithDescription("Updates an existing user")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/users/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .WithDescription("Deletes a user")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Tenant management endpoints
        group.MapGet("/tenants", GetTenants)
            .WithName("GetTenants")
            .WithSummary("Get all tenants")
            .WithDescription("Retrieves all tenants (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces<PaginatedResult<TenantDto>>(StatusCodes.Status200OK);

        group.MapGet("/tenants/{id:guid}", GetTenant)
            .WithName("GetTenant")
            .WithSummary("Get tenant by ID")
            .WithDescription("Retrieves a specific tenant")
            .Produces<TenantDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/tenants", CreateTenant)
            .WithName("CreateTenant")
            .WithSummary("Create a new tenant")
            .WithDescription("Creates a new tenant (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces<TenantDto>(StatusCodes.Status201Created);

        group.MapPut("/tenants/{id:guid}", UpdateTenant)
            .WithName("UpdateTenant")
            .WithSummary("Update a tenant")
            .WithDescription("Updates an existing tenant")
            .Produces<TenantDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/tenants/{id:guid}", DeleteTenant)
            .WithName("DeleteTenant")
            .WithSummary("Delete a tenant")
            .WithDescription("Deletes a tenant (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // AI Configuration endpoints
        group.MapGet("/ai-configuration", GetAiConfiguration)
            .WithName("GetAiConfiguration")
            .WithSummary("Get AI configuration")
            .WithDescription("Retrieves the AI configuration for the current tenant")
            .Produces<AiConfigurationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/ai-configuration", UpdateAiConfiguration)
            .WithName("UpdateAiConfiguration")
            .WithSummary("Update AI configuration")
            .WithDescription("Updates the AI configuration for the current tenant")
            .Produces<AiConfigurationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // FAQ management endpoints
        group.MapGet("/faqs", GetFaqs)
            .WithName("GetFaqs")
            .WithSummary("Get FAQ entries")
            .WithDescription("Retrieves FAQ entries for the current tenant")
            .Produces<PaginatedResult<FaqEntryDto>>(StatusCodes.Status200OK);

        group.MapGet("/faqs/{id:guid}", GetFaq)
            .WithName("GetFaq")
            .WithSummary("Get FAQ entry by ID")
            .WithDescription("Retrieves a specific FAQ entry")
            .Produces<FaqEntryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/faqs", CreateFaq)
            .WithName("CreateFaq")
            .WithSummary("Create a new FAQ entry")
            .WithDescription("Creates a new FAQ entry for the current tenant")
            .Produces<FaqEntryDto>(StatusCodes.Status201Created);

        group.MapPut("/faqs/{id:guid}", UpdateFaq)
            .WithName("UpdateFaq")
            .WithSummary("Update a FAQ entry")
            .WithDescription("Updates an existing FAQ entry")
            .Produces<FaqEntryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/faqs/{id:guid}", DeleteFaq)
            .WithName("DeleteFaq")
            .WithSummary("Delete a FAQ entry")
            .WithDescription("Deletes a FAQ entry")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Training files endpoints
        group.MapGet("/training-files", GetTrainingFiles)
            .WithName("GetTrainingFiles")
            .WithSummary("Get training files")
            .WithDescription("Retrieves AI training files for the current tenant")
            .Produces<PaginatedResult<AiTrainingFileDto>>(StatusCodes.Status200OK);

        group.MapGet("/training-files/{id:guid}", GetTrainingFile)
            .WithName("GetTrainingFile")
            .WithSummary("Get training file by ID")
            .WithDescription("Retrieves a specific AI training file")
            .Produces<AiTrainingFileDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/training-files", UploadTrainingFile)
            .WithName("UploadTrainingFile")
            .WithSummary("Upload a new training file")
            .WithDescription("Uploads a new AI training file for the current tenant")
            .Accepts<UploadTrainingFileCommand>("multipart/form-data")
            .Produces<AiTrainingFileDto>(StatusCodes.Status201Created);

        group.MapDelete("/training-files/{id:guid}", DeleteTrainingFile)
            .WithName("DeleteTrainingFile")
            .WithSummary("Delete a training file")
            .WithDescription("Deletes an AI training file")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
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

    private static async Task<IResult> GetAiConfiguration(ISender sender)
    {
        var query = new GetAIConfigurationQuery();
        var result = await sender.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateAiConfiguration(UpdateAiConfigurationCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetFaqs([AsParameters] PaginationRequest request, ISender sender)
    {
        var query = new GetAllFaqEntriesQuery { Pagination = request };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFaq(Guid id, ISender sender)
    {
        var query = new GetFaqEntryByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateFaq(CreateFaqEntryCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return Results.Created($"/api/v1/admin/faqs/{result.Id}", result);
    }

    private static async Task<IResult> UpdateFaq(Guid id, UpdateFaqEntryCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("ID mismatch");
        }
        var result = await sender.Send(command);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteFaq(Guid id, ISender sender)
    {
        var command = new DeleteFaqEntryCommand { Id = id };
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> GetTrainingFiles([AsParameters] PaginationRequest request, ISender sender)
    {
        var query = new GetAllTrainingFilesQuery { Pagination = request };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTrainingFile(Guid id, ISender sender)
    {
        var query = new GetTrainingFileByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UploadTrainingFile([FromForm] IFormFile file, [FromForm] string? description, [FromForm] List<string>? tags, ISender sender)
    {
        var command = new UploadTrainingFileCommand { File = file, Description = description, Tags = tags };
        var result = await sender.Send(command);
        return Results.Created($"/api/v1/admin/training-files/{result.Id}", result);
    }

    private static async Task<IResult> DeleteTrainingFile(Guid id, ISender sender)
    {
        var command = new DeleteTrainingFileCommand { Id = id };
        await sender.Send(command);
        return Results.NoContent();
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

    private static async Task<IResult> CreateUser(CreateUserCommand command, ISender sender)
    {
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