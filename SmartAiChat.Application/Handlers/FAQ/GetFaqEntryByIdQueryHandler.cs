using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class GetFaqEntryByIdQueryHandler : IRequestHandler<GetFaqEntryByIdQuery, FaqEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public GetFaqEntryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<FaqEntryDto> Handle(GetFaqEntryByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.GetTenantId();
            var faqEntry = await _unitOfWork.FaqEntries.GetFirstOrDefaultAsync(
                filter: f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken: cancellationToken);

            if (faqEntry == null)
            {
                return null;
            }

            return _mapper.Map<FaqEntryDto>(faqEntry);
        }
    }
}
