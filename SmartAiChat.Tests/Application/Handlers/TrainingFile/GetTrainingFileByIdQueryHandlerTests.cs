using AutoMapper;
using Moq;
using SmartAiChat.Application.Handlers.TrainingFile;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.TrainingFile
{
    public class GetTrainingFileByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GetTrainingFileByIdQueryHandler _handler;

        public GetTrainingFileByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new GetTrainingFileByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrainingFile_WhenFileExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var query = new GetTrainingFileByIdQuery { Id = Guid.NewGuid() };
            var trainingFile = new AiTrainingFile { Id = query.Id, TenantId = tenantId };
            var trainingFileDto = new SmartAiChat.Application.DTOs.AiTrainingFileDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiTrainingFiles.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiTrainingFile, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(trainingFile);
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.AiTrainingFileDto>(trainingFile)).Returns(trainingFileDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trainingFileDto, result);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var query = new GetTrainingFileByIdQuery { Id = Guid.NewGuid() };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiTrainingFiles.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiTrainingFile, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((AiTrainingFile)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
