using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Queries.TrainingFile
{
    public class GetTrainingFileByIdQuery : IRequest<AiTrainingFileDto>
    {
        public Guid Id { get; set; }
    }
}
