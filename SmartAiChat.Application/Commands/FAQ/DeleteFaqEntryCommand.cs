using MediatR;

namespace SmartAiChat.Application.Commands.FAQ
{
    public class DeleteFaqEntryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
