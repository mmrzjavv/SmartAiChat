using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Queries.FAQ
{
    public class GetFaqEntryByIdQuery : IRequest<FaqEntryDto>
    {
        public Guid Id { get; set; }
    }
}
