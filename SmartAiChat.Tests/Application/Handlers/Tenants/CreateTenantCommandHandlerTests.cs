using AutoMapper;
using Moq;
using SmartAiChat.Application.Commands.Tenants;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Handlers.Tenants;
using SmartAiChat.Application.Mappings;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.Tenants;

public class CreateTenantCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;

    public CreateTenantCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfile());
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_Should_CreateTenant()
    {
        // Arrange
        var command = new CreateTenantCommand
        {
            Name = "Test Tenant",
            Domain = "test.com",
            Email = "test@test.com"
        };

        var repositoryMock = new Mock<IRepository<Tenant>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tenant>(), default))
            .ReturnsAsync((Tenant t, CancellationToken ct) => t);

        _unitOfWorkMock.Setup(uow => uow.Tenants).Returns(repositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new CreateTenantCommandHandler(_unitOfWorkMock.Object, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Domain, result.Domain);
        Assert.Equal(command.Email, result.Email);

        repositoryMock.Verify(r => r.AddAsync(It.Is<Tenant>(t => t.Name == command.Name), default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }
}
