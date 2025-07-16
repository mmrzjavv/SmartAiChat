using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Commands.FAQ
{
    public class CreateFaqEntryCommand : IRequest<FaqEntryDto>
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string? Category { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public string Language { get; set; }
    }
}
