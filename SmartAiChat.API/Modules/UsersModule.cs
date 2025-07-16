using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.Queries.Users;
using System.Security.Claims;

namespace SmartAiChat.API.Modules;

public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();

        // Get current user's profile
        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current user's profile")
            .WithDescription("Retrieves the profile of the currently authenticated user");

        // Update current user's profile
        group.MapPut("/me", UpdateCurrentUser)
            .WithName("UpdateCurrentUser")
            .WithSummary("Update current user's profile")
            .WithDescription("Updates the profile of the currently authenticated user");

        // Delete current user's account
        group.MapDelete("/me", DeleteCurrentUser)
            .WithName("DeleteCurrentUser")
            .WithSummary("Delete current user's account")
            .WithDescription("Deletes the currently authenticated user's account");
    }

    private static async Task<IResult> GetCurrentUser(ISender sender, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();
        var query = new GetUserByIdQuery { Id = Guid.Parse(userId) };
        var result = await sender.Send(query);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateCurrentUser([FromBody] UpdateUserCommand command, ISender sender, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();
        command.Id = Guid.Parse(userId);
        var result = await sender.Send(command);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteCurrentUser(ISender sender, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();
        var command = new DeleteUserCommand { Id = Guid.Parse(userId) };
        await sender.Send(command);
        return Results.NoContent();
    }
} 