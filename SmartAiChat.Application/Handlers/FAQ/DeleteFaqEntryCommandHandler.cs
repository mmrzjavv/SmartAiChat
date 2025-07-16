using MediatR;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.FAQ
{
    public class DeleteFaqEntryCommandHandler : IRequestHandler<DeleteFaqEntryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantContext _tenantContext;

        public DeleteFaqEntryCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _tenantContext = tenantContext;
        }

        public async Task<Unit> Handle(DeleteFaqEntryCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.GetTenantId();
            var faqEntry = await _unitOfWork.FaqEntries.GetFirstOrDefaultAsync(
                filter: f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken: cancellationToken);

            if (faqEntry == null)
            {
                throw new Exception("FAQ Entry not found.");
            }

            _unitOfWork.FaqEntries.Remove(faqEntry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
