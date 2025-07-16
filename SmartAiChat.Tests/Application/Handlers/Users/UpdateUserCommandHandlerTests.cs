using AutoMapper;
using FluentAssertions;
using Moq;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Exceptions;
using SmartAiChat.Application.Handlers.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Tests.Application.Handlers.Users;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserAndReturnUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = userId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        var user = new User { Id = userId };
        var userDto = new UserDto { Id = userId };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Repository<User>().UpdateAsync(user))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
        user.FirstName.Should().Be(command.FirstName);
        user.LastName.Should().Be(command.LastName);
        user.Email.Should().Be(command.Email);
        user.Role.Should().Be(command.Role);
        user.IsActive.Should().Be(command.IsActive);
        _unitOfWorkMock.Verify(uow => uow.Repository<User>().UpdateAsync(user), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand { Id = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(command.Id))
            .ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
