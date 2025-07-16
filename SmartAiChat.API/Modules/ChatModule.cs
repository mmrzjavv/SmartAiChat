using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.ChatSessions;
using SmartAiChat.Application.Commands.ChatMessages;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.ChatSessions;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.API.Modules;

public class ChatModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/chat")
            .WithTags("Chat")
            .WithOpenApi();

        // Start a new chat session
        group.MapPost("/sessions", StartChatSession)
            .WithName("StartChatSession")
            .WithSummary("Start a new chat session")
            .WithDescription("Creates a new chat session for a customer")
            .Produces<ApiResponse<ChatSessionDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ChatSessionDto>>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        // Get a chat session by ID
        group.MapGet("/sessions/{id:guid}", GetChatSession)
            .WithName("GetChatSession")
            .WithSummary("Get a chat session by ID")
            .WithDescription("Retrieves a specific chat session")
            .Produces<ApiResponse<ChatSessionDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ChatSessionDto>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Send a message in a chat session
        group.MapPost("/messages", SendMessage)
            .WithName("SendMessage")
            .WithSummary("Send a message in a chat session")
            .WithDescription("Sends a message to a chat session")
            .Produces<ApiResponse<ChatMessageDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ChatMessageDto>>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        // Get health status
        group.MapGet("/health", GetHealth)
            .WithName("GetChatHealth")
            .WithSummary("Get health status")
            .WithDescription("Returns the health status of the chat service")
            .Produces<object>(StatusCodes.Status200OK)
            .AllowAnonymous();
    }

    private static async Task<IResult> StartChatSession(
        [FromBody] StartChatSessionCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetChatSession(
        Guid id,
        IMediator mediator)
    {
        var query = new GetChatSessionQuery(id);
        var result = await mediator.Send(query);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.NotFound(result);
    }

    private static async Task<IResult> SendMessage(
        [FromBody] SendMessageCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.BadRequest(result);
    }

    private static IResult GetHealth()
    {
        return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
} 