using AutoMapper;
using FluentAssertions;
using Moq;
using SmartAiChat.Application.Commands.ChatSessions;
using SmartAiChat.Application.Handlers.ChatSessions;
using SmartAiChat.Application.Mappings;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Tests.Application.Handlers;

public class StartChatSessionHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly IMapper _mapper;
    private readonly StartChatSessionHandler _handler;

    public StartChatSessionHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = configuration.CreateMapper();
        
        _handler = new StartChatSessionHandler(_mockUnitOfWork.Object, _mapper, _mockTenantContext.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResponse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        
        var command = new StartChatSessionCommand
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Subject = "Test Subject"
        };

        var customer = new User
        {
            Id = customerId,
            TenantId = tenantId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Role = UserRole.Customer
        };

        var chatSession = new ChatSession
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = customerId,
            SessionId = "12345678",
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com"
        };

        _mockTenantContext.Setup(x => x.TenantId).Returns(tenantId);
        
        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUnitOfWork.Setup(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _mockUnitOfWork.Setup(x => x.ChatSessions.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatSession);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CustomerName.Should().Be("John Doe");
        result.Data.CustomerEmail.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Handle_ExistingCustomer_ReusesCustomer()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        
        var command = new StartChatSessionCommand
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com"
        };

        var existingCustomer = new User
        {
            Id = customerId,
            TenantId = tenantId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Role = UserRole.Customer
        };

        var chatSession = new ChatSession
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = customerId,
            SessionId = "12345678",
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com"
        };

        _mockTenantContext.Setup(x => x.TenantId).Returns(tenantId);
        
        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        _mockUnitOfWork.Setup(x => x.ChatSessions.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatSession);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        // Verify that no new user was created
        _mockUnitOfWork.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
} 