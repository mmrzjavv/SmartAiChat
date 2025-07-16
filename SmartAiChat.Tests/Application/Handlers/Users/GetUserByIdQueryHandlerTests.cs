using AutoMapper;
using FluentAssertions;
using Moq;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Exceptions;
using SmartAiChat.Application.Handlers.Users;
using SmartAiChat.Application.Queries.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.Users;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery { Id = userId };
        var user = new User { Id = userId };
        var userDto = new UserDto { Id = userId };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var query = new GetUserByIdQuery { Id = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetByIdAsync(query.Id))
            .ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
