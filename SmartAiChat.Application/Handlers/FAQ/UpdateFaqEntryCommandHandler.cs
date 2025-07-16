using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Interfaces;
using System.Text.Json;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class UpdateFaqEntryCommandHandler : IRequestHandler<UpdateFaqEntryCommand, FaqEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public UpdateFaqEntryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<FaqEntryDto> Handle(UpdateFaqEntryCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var faqEntry = await _unitOfWork.FaqEntries.FirstOrDefaultAsync(
                f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken);

            if (faqEntry == null)
            {
                throw new Exception("FAQ Entry not found.");
            }

            _mapper.Map(request, faqEntry);
            if (request.Tags != null)
            {
                faqEntry.Tags = JsonSerializer.Serialize(request.Tags);
            }

            await _unitOfWork.FaqEntries.UpdateAsync(faqEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FaqEntryDto>(faqEntry);
        }
    }
}
