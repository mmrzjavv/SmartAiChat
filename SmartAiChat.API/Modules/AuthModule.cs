using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Authentication;

namespace SmartAiChat.API.Modules;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticates a user and returns tokens")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        group.MapPost("/refresh-token", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("Refresh authentication token")
            .WithDescription("Requests a new access token using a refresh token")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.Unauthorized();
    }

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.Unauthorized();
    }
} 