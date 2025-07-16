using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.Users;
using SmartAiChat.Shared.Models;
using System;
using System.Threading.Tasks;

namespace SmartAiChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
        {
            var query = new GetAllUsersQuery { Pagination = request };
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _sender.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _sender.Send(command);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand { Id = id };
            await _sender.Send(command);
            return NoContent();
        }
    }
}
