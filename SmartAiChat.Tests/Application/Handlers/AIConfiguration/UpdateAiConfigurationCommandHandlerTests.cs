using AutoMapper;
using Moq;
using SmartAiChat.Application.Commands.AIConfiguration;
using SmartAiChat.Application.Handlers.AIConfiguration;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.AIConfiguration
{
    public class UpdateAiConfigurationCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly UpdateAiConfigurationCommandHandler _handler;

        public UpdateAiConfigurationCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new UpdateAiConfigurationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAiConfiguration_WhenConfigurationExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new UpdateAiConfigurationCommand();
            var aiConfiguration = new AiConfiguration { TenantId = tenantId };
            var aiConfigurationDto = new SmartAiChat.Application.DTOs.AiConfigurationDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiConfigurations.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiConfiguration, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiConfiguration);
            _mapperMock.Setup(m => m.Map(command, aiConfiguration));
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.AiConfigurationDto>(aiConfiguration)).Returns(aiConfigurationDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.AiConfigurations.Update(aiConfiguration), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(aiConfigurationDto, result);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenConfigurationDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new UpdateAiConfigurationCommand();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiConfigurations.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiConfiguration, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((AiConfiguration)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
