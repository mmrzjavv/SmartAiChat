using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.Tenants;
using SmartAiChat.Shared.Models;
using System;
using System.Threading.Tasks;

namespace SmartAiChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ISender _sender;

        public TenantController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<TenantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTenants([FromQuery] PaginationRequest request)
        {
            var query = new GetAllTenantsQuery { Pagination = request };
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTenant(Guid id)
        {
            var query = new GetTenantByIdQuery { Id = id };
            var result = await _sender.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTenant(CreateTenantCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetTenant), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTenant(Guid id, UpdateTenantCommand command)
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
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var command = new DeleteTenantCommand { Id = id };
            await _sender.Send(command);
            return NoContent();
        }

        // In a real application, you would have more endpoints for managing tenant subscriptions,
        // such as creating, updating, and canceling subscriptions.
        // For now, these are the basic endpoints.
    }
}
