using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Authentication;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.API.Modules;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", Register)
            .WithName("AuthRegister")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account")
            .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        group.MapPost("/login", Login)
            .WithName("AuthLogin")
            .WithSummary("User login")
            .WithDescription("Authenticates a user and returns tokens")
            .Produces<ApiResponse<LoginResponseDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        group.MapPost("/refresh-token", RefreshToken)
            .WithName("AuthRefreshToken")
            .WithSummary("Refresh authentication token")
            .WithDescription("Requests a new access token using a refresh token")
            .Produces<ApiResponse<LoginResponseDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.Unauthorized();
    }

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.Unauthorized();
    }
}