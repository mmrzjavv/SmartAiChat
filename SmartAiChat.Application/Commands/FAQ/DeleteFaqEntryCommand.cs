using MediatR;

namespace SmartAiChat.Application.Commands.FAQ
{
    public class DeleteFaqEntryCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
