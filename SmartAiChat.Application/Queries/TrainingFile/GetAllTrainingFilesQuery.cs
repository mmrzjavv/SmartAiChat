using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Queries.TrainingFile
{
    public class GetAllTrainingFilesQuery : IRequest<PaginatedResult<AiTrainingFileDto>>
    {
        public PaginationRequest Pagination { get; set; }
    }
}
