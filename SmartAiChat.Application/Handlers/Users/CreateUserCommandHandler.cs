using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            TenantId = request.TenantId
        };

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserDto>(user);
    }
}
