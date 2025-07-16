using MediatR;

namespace SmartAiChat.Application.Commands.TrainingFile
{
    public class DeleteTrainingFileCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
