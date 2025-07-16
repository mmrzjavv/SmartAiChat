using MediatR;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.Tenants;

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Tenants.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
