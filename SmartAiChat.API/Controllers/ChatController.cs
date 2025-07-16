using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.ChatSessions;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.ChatSessions;
using System;
using System.Threading.Tasks;

namespace SmartAiChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ISender _sender;

        public ChatController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("start")]
        [ProducesResponseType(typeof(ChatSessionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartChatSession(StartChatSessionCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChatSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChatSession(Guid id)
        {
            var query = new GetChatSessionQuery { Id = id };
            var result = await _sender.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        // In a real application, you would have more endpoints for managing chat sessions,
        // such as updating the status, assigning an operator, etc.
        // For now, these are the basic endpoints.
    }
}
