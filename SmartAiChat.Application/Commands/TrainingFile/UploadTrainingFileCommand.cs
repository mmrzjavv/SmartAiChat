using MediatR;
using Microsoft.AspNetCore.Http;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Commands.TrainingFile
{
    public class UploadTrainingFileCommand : IRequest<AiTrainingFileDto>
    {
        public IFormFile File { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
    }
}
