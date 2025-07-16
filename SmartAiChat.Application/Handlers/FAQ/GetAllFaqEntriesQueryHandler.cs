using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class GetAllFaqEntriesQueryHandler : IRequestHandler<GetAllFaqEntriesQuery, PaginatedResult<FaqEntryDto>>
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

        public async Task<PaginatedResult<FaqEntryDto>> Handle(GetAllFaqEntriesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.GetTenantId();
            var (faqEntries, totalCount) = await _unitOfWork.FaqEntries.GetAsync(
                filter: f => f.TenantId == tenantId,
                page: request.Pagination.Page,
                pageSize: request.Pagination.PageSize,
                cancellationToken: cancellationToken);

            var faqEntryDtos = _mapper.Map<IEnumerable<FaqEntryDto>>(faqEntries);

            return new PaginatedResult<FaqEntryDto>(
                faqEntryDtos,
                totalCount,
                request.Pagination.Page,
                request.Pagination.PageSize);
        }
    }
}
