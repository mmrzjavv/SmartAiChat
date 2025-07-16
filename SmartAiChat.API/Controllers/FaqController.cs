using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAiChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly ISender _sender;

        public FaqController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<FaqEntryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFaqs([FromQuery] PaginationRequest request)
        {
            var query = new GetAllFaqEntriesQuery { Pagination = request };
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FaqEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFaq(Guid id)
        {
            var query = new GetFaqEntryByIdQuery { Id = id };
            var result = await _sender.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(FaqEntryDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateFaq(CreateFaqEntryCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetFaq), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FaqEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFaq(Guid id, UpdateFaqEntryCommand command)
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
        public async Task<IActionResult> DeleteFaq(Guid id)
        {
            var command = new DeleteFaqEntryCommand { Id = id };
            await _sender.Send(command);
            return NoContent();
        }

        [HttpGet("training-files")]
        [ProducesResponseType(typeof(PaginatedResult<AiTrainingFileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrainingFiles([FromQuery] PaginationRequest request)
        {
            var query = new GetAllTrainingFilesQuery { Pagination = request };
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("training-files/{id}")]
        [ProducesResponseType(typeof(AiTrainingFileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrainingFile(Guid id)
        {
            var query = new GetTrainingFileByIdQuery { Id = id };
            var result = await _sender.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPost("training-files")]
        [ProducesResponseType(typeof(AiTrainingFileDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadTrainingFile([FromForm] IFormFile file, [FromForm] string? description, [FromForm] List<string>? tags)
        {
            var command = new UploadTrainingFileCommand { File = file, Description = description, Tags = tags };
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetTrainingFile), new { id = result.Id }, result);
        }

        [HttpDelete("training-files/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTrainingFile(Guid id)
        {
            var command = new DeleteTrainingFileCommand { Id = id };
            await _sender.Send(command);
            return NoContent();
        }
    }
}
