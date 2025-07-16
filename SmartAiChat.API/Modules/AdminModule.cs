using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.AIConfiguration;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.DTOs;
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
            .WithName("GetAdminUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves all users for the current tenant")
            .Produces<ApiResponse<PaginatedResponse<UserDto>>>();

        group.MapGet("/users/{id:guid}", GetUser)
            .WithName("GetAdminUser")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a specific user")
            .Produces<ApiResponse<UserDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/users", CreateUser)
            .WithName("CreateAdminUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user for the current tenant")
            .Produces<ApiResponse<UserDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/users/{id:guid}", UpdateUser)
            .WithName("UpdateAdminUser")
            .WithSummary("Update a user")
            .WithDescription("Updates an existing user")
            .Produces<ApiResponse<UserDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/users/{id:guid}", DeleteUser)
            .WithName("DeleteAdminUser")
            .WithSummary("Delete a user")
            .WithDescription("Deletes a user")
            .Produces<ApiResponse>(StatusCodes.Status204NoContent)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        // Tenant management endpoints
        group.MapGet("/tenants", GetTenants)
            .WithName("GetAdminTenants")
            .WithSummary("Get all tenants")
            .WithDescription("Retrieves all tenants (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces<ApiResponse<PaginatedResponse<TenantDto>>>();

        group.MapGet("/tenants/{id:guid}", GetTenant)
            .WithName("GetAdminTenant")
            .WithSummary("Get tenant by ID")
            .WithDescription("Retrieves a specific tenant")
            .Produces<ApiResponse<TenantDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/tenants", CreateTenant)
            .WithName("CreateAdminTenant")
            .WithSummary("Create a new tenant")
            .WithDescription("Creates a new tenant (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces<ApiResponse<TenantDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/tenants/{id:guid}", UpdateTenant)
            .WithName("UpdateAdminTenant")
            .WithSummary("Update a tenant")
            .WithDescription("Updates an existing tenant")
            .Produces<ApiResponse<TenantDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/tenants/{id:guid}", DeleteTenant)
            .WithName("DeleteAdminTenant")
            .WithSummary("Delete a tenant")
            .WithDescription("Deletes a tenant (SuperAdmin only)")
            .RequireAuthorization(SystemConstants.Policies.SuperAdminOnly)
            .Produces<ApiResponse>(StatusCodes.Status204NoContent)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        // AI Configuration endpoints
        group.MapGet("/ai-configuration", GetAiConfiguration)
            .WithName("GetAdminAiConfiguration")
            .WithSummary("Get AI configuration")
            .WithDescription("Retrieves the AI configuration for the current tenant")
            .Produces<ApiResponse<AiConfigurationDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/ai-configuration", UpdateAiConfiguration)
            .WithName("UpdateAdminAiConfiguration")
            .WithSummary("Update AI configuration")
            .WithDescription("Updates the AI configuration for the current tenant")
            .Produces<ApiResponse<AiConfigurationDto>>()
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        // FAQ management endpoints
        group.MapGet("/faqs", GetFaqs)
            .WithName("GetAdminFaqs")
            .WithSummary("Get FAQ entries")
            .WithDescription("Retrieves FAQ entries for the current tenant")
            .Produces<ApiResponse<PaginatedResponse<FaqEntryDto>>>();

        group.MapGet("/faqs/{id:guid}", GetFaq)
            .WithName("GetAdminFaq")
            .WithSummary("Get FAQ entry by ID")
            .WithDescription("Retrieves a specific FAQ entry")
            .Produces<ApiResponse<FaqEntryDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/faqs", CreateFaq)
            .WithName("CreateAdminFaq")
            .WithSummary("Create a new FAQ entry")
            .WithDescription("Creates a new FAQ entry for the current tenant")
            .Produces<ApiResponse<FaqEntryDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/faqs/{id:guid}", UpdateFaq)
            .WithName("UpdateAdminFaq")
            .WithSummary("Update a FAQ entry")
            .WithDescription("Updates an existing FAQ entry")
            .Produces<ApiResponse<FaqEntryDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/faqs/{id:guid}", DeleteFaq)
            .WithName("DeleteAdminFaq")
            .WithSummary("Delete a FAQ entry")
            .WithDescription("Deletes a FAQ entry")
            .Produces<ApiResponse>(StatusCodes.Status204NoContent)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        // Training files endpoints
        group.MapGet("/training-files", GetTrainingFiles)
            .WithName("GetAdminTrainingFiles")
            .WithSummary("Get training files")
            .WithDescription("Retrieves AI training files for the current tenant")
            .Produces<ApiResponse<PaginatedResponse<AiTrainingFileDto>>>();

        group.MapGet("/training-files/{id:guid}", GetTrainingFile)
            .WithName("GetAdminTrainingFile")
            .WithSummary("Get training file by ID")
            .WithDescription("Retrieves a specific AI training file")
            .Produces<ApiResponse<AiTrainingFileDto>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/training-files", UploadTrainingFile)
            .WithName("UploadAdminTrainingFile")
            .WithSummary("Upload a new training file")
            .WithDescription("Uploads a new AI training file for the current tenant")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ApiResponse<AiTrainingFileDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/training-files/{id:guid}", DeleteTrainingFile)
            .WithName("DeleteAdminTrainingFile")
            .WithSummary("Delete a training file")
            .WithDescription("Deletes an AI training file")
            .Produces<ApiResponse>(StatusCodes.Status204NoContent)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetTenants([AsParameters] PaginationRequest request, ISender sender)
    {
        try
        {
            request.EnsureValidPagination();
            var query = new GetAllTenantsQuery { Pagination = request };
            var result = await sender.Send(query);
            var response = ApiResponse<PaginatedResponse<TenantDto>>.Success(result, "Tenants retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetTenant(Guid id, ISender sender)
    {
        try
        {
            var query = new GetTenantByIdQuery { Id = id };
            var result = await sender.Send(query);
            var response = ApiResponse<TenantDto>.Success(result, "Tenant retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> CreateTenant([FromBody] CreateTenantCommand command, ISender sender)
    {
        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<TenantDto>.Success(result, "Tenant created successfully");
            return Results.Created($"/api/v1/admin/tenants/{result.Id}", response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            var badRequestResponse = ApiResponse.Failure("ID mismatch");
            return Results.BadRequest(badRequestResponse);
        }

        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<TenantDto>.Success(result, "Tenant updated successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> DeleteTenant(Guid id, ISender sender)
    {
        try
        {
            var command = new DeleteTenantCommand { Id = id };
            await sender.Send(command);
            ApiResponse.Success("Tenant deleted successfully");
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetAiConfiguration(ISender sender)
    {
        try
        {
            var query = new GetAIConfigurationQuery();
            var result = await sender.Send(query);
            var response = ApiResponse<AiConfigurationDto>.Success(result, "AI configuration retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> UpdateAiConfiguration([FromBody] UpdateAiConfigurationCommand command, ISender sender)
    {
        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<AiConfigurationDto>.Success(result, "AI configuration updated successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetFaqs([AsParameters] PaginationRequest request, ISender sender)
    {
        try
        {
            request.EnsureValidPagination();
            var query = new GetAllFaqEntriesQuery { Pagination = request };
            var result = await sender.Send(query);
            var response = ApiResponse<PaginatedResponse<FaqEntryDto>>.Success(result, "FAQ entries retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetFaq(Guid id, ISender sender)
    {
        try
        {
            var query = new GetFaqEntryByIdQuery { Id = id };
            var result = await sender.Send(query);
            var response = ApiResponse<FaqEntryDto>.Success(result, "FAQ entry retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> CreateFaq([FromBody] CreateFaqEntryCommand command, ISender sender)
    {
        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<FaqEntryDto>.Success(result, "FAQ entry created successfully");
            return Results.Created($"/api/v1/admin/faqs/{result.Id}", response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> UpdateFaq(Guid id, [FromBody] UpdateFaqEntryCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            var badRequestResponse = ApiResponse.Failure("ID mismatch");
            return Results.BadRequest(badRequestResponse);
        }

        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<FaqEntryDto>.Success(result, "FAQ entry updated successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> DeleteFaq(Guid id, ISender sender)
    {
        try
        {
            var command = new DeleteFaqEntryCommand { Id = id };
            await sender.Send(command);
            ApiResponse.Success("FAQ entry deleted successfully");
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetTrainingFiles([AsParameters] PaginationRequest request, ISender sender)
    {
        try
        {
            request.EnsureValidPagination();
            var query = new GetAllTrainingFilesQuery { Pagination = request };
            var result = await sender.Send(query);
            var response = ApiResponse<PaginatedResponse<AiTrainingFileDto>>.Success(result, "Training files retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetTrainingFile(Guid id, ISender sender)
    {
        try
        {
            var query = new GetTrainingFileByIdQuery { Id = id };
            var result = await sender.Send(query);
            var response = ApiResponse<AiTrainingFileDto>.Success(result, "Training file retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> UploadTrainingFile([FromForm] IFormFile file, [FromForm] string? description, [FromForm] List<string>? tags, ISender sender)
    {
        try
        {
            var command = new UploadTrainingFileCommand
            {
                File = file,
                Description = description,
                Tags = tags
            };
            var result = await sender.Send(command);
            var response = ApiResponse<AiTrainingFileDto>.Success(result, "Training file uploaded successfully");
            return Results.Created($"/api/v1/admin/training-files/{result.Id}", response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> DeleteTrainingFile(Guid id, ISender sender)
    {
        try
        {
            var command = new DeleteTrainingFileCommand { Id = id };
            await sender.Send(command);
            ApiResponse.Success("Training file deleted successfully");
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetUsers([AsParameters] PaginationRequest request, ISender sender)
    {
        try
        {
            request.EnsureValidPagination();
            var query = new GetAllUsersQuery { Pagination = request };
            var result = await sender.Send(query);
            var response = ApiResponse<PaginatedResponse<UserDto>>.Success(result, "Users retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> GetUser(Guid id, ISender sender)
    {
        try
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await sender.Send(query);
            var response = ApiResponse<UserDto>.Success(result, "User retrieved successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> CreateUser([FromBody] CreateUserCommand command, ISender sender)
    {
        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<UserDto>.Success(result, "User created successfully");
            return Results.Created($"/api/v1/admin/users/{result.Id}", response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command, ISender sender)
    {
        if (id != command.Id)
        {
            var badRequestResponse = ApiResponse.Failure("ID mismatch");
            return Results.BadRequest(badRequestResponse);
        }

        try
        {
            var result = await sender.Send(command);
            var response = ApiResponse<UserDto>.Success(result, "User updated successfully");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }

    private static async Task<IResult> DeleteUser(Guid id, ISender sender)
    {
        try
        {
            var command = new DeleteUserCommand { Id = id };
            await sender.Send(command);
            ApiResponse.Success("User deleted successfully");
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            var response = ApiResponse.Failure(ex.Message);
            return Results.BadRequest(response);
        }
    }
}
