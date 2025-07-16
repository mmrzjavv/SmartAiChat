using MediatR;

namespace SmartAiChat.Application.Commands.TrainingFile
{
    public class DeleteTrainingFileCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
