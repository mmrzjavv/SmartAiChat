using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Queries.FAQ
{
    public class GetAllFaqEntriesQuery : IRequest<PaginatedResponse<FaqEntryDto>>
    {
        public PaginationRequest Pagination { get; set; }
    }
}
