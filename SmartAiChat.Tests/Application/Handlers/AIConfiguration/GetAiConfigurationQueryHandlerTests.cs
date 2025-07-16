using AutoMapper;
using Moq;
using SmartAiChat.Application.Handlers.AIConfiguration;
using SmartAiChat.Application.Queries.AIConfiguration;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.AIConfiguration
{
    public class GetAiConfigurationQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GetAiConfigurationQueryHandler _handler;

        public GetAiConfigurationQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new GetAiConfigurationQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAiConfiguration_WhenConfigurationExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var aiConfiguration = new AiConfiguration { TenantId = tenantId };
            var aiConfigurationDto = new SmartAiChat.Application.DTOs.AiConfigurationDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiConfigurations.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiConfiguration, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiConfiguration);
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.AiConfigurationDto>(aiConfiguration)).Returns(aiConfigurationDto);

            // Act
            var result = await _handler.Handle(new GetAIConfigurationQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(aiConfigurationDto, result);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenConfigurationDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiConfigurations.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiConfiguration, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((AiConfiguration)null);

            // Act
            var result = await _handler.Handle(new GetAIConfigurationQuery(), CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
