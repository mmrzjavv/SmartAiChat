using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class GetAllFaqEntriesQueryHandler : IRequestHandler<GetAllFaqEntriesQuery, PaginatedResponse<FaqEntryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public GetAllFaqEntriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<PaginatedResponse<FaqEntryDto>> Handle(GetAllFaqEntriesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var pagedResult = await _unitOfWork.FaqEntries.GetPagedAsync(request.Pagination, cancellationToken);
            var faqEntryDtos = _mapper.Map<IEnumerable<FaqEntryDto>>(pagedResult.Items);
            return PaginatedResponse<FaqEntryDto>.Create(
                faqEntryDtos.ToList(),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize);
        }
    }
}
