using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System.Text.Json;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class CreateFaqEntryCommandHandler : IRequestHandler<CreateFaqEntryCommand, FaqEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public CreateFaqEntryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<FaqEntryDto> Handle(CreateFaqEntryCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var faqEntry = _mapper.Map<FaqEntry>(request);
            faqEntry.TenantId = tenantId;
            if (request.Tags != null)
            {
                faqEntry.Tags = JsonSerializer.Serialize(request.Tags);
            }

            _unitOfWork.FaqEntries.Add<FaqEntry>(faqEntry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FaqEntryDto>(faqEntry);
        }
    }
}
