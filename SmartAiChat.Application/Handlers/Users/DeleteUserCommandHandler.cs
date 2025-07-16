using MediatR;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.Exceptions;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.Users;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        await _unitOfWork.Repository<User>().DeleteAsync(user);
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}
