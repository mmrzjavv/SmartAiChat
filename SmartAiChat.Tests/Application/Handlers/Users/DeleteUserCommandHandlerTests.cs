using FluentAssertions;
using MediatR;
using Moq;
using SmartAiChat.Application.Commands.Users;
using SmartAiChat.Application.Exceptions;
using SmartAiChat.Application.Handlers.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.Users;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteUserCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };
        var user = new User { Id = userId };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Repository<User>().DeleteAsync(user))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _unitOfWorkMock.Verify(uow => uow.Repository<User>().DeleteAsync(user), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new DeleteUserCommand { Id = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(command.Id))
            .ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
