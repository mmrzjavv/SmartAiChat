using AutoMapper;
using FluentAssertions;
using Moq;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Handlers.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Tests.Application.Handlers.Users;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnUserDto()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password",
            Role = UserRole.User
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Role = command.Role
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(uow => uow.TenantContext.GetTenantId()).Returns(Guid.NewGuid());
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
        _unitOfWorkMock.Verify(uow => uow.Repository<User>().AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
}
