using AutoMapper;
using FluentAssertions;
using Moq;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Handlers.Users;
using SmartAiChat.Application.Queries.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.Users;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllUsersQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfUserDtos()
    {
        // Arrange
        var query = new GetAllUsersQuery();
        var users = new List<User> { new User(), new User() };
        var userDtos = new List<UserDto> { new UserDto(), new UserDto() };
        var tenantId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.TenantContext.GetTenantId()).Returns(tenantId);
        _unitOfWorkMock.Setup(uow => uow.Repository<User>().GetAllAsync(
                query.Pagination.Page,
                query.Pagination.PageSize,
                u => u.TenantId == tenantId))
            .ReturnsAsync((users, users.Count));
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }
}
