using Moq;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.Handlers.TrainingFile;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.TrainingFile
{
    public class DeleteTrainingFileCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly Mock<IFileProcessingService> _fileProcessingServiceMock;
        private readonly DeleteTrainingFileCommandHandler _handler;

        public DeleteTrainingFileCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tenantContextMock = new Mock<ITenantContext>();
            _fileProcessingServiceMock = new Mock<IFileProcessingService>();
            _handler = new DeleteTrainingFileCommandHandler(
                _unitOfWorkMock.Object,
                _tenantContextMock.Object,
                _fileProcessingServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteTrainingFile_WhenFileExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DeleteTrainingFileCommand { Id = Guid.NewGuid() };
            var trainingFile = new AiTrainingFile { Id = command.Id, TenantId = tenantId, FilePath = "path/to/file" };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiTrainingFiles.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiTrainingFile, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(trainingFile);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _fileProcessingServiceMock.Verify(f => f.DeleteFileAsync(trainingFile.FilePath), Times.Once);
            _unitOfWorkMock.Verify(u => u.AiTrainingFiles.Remove(trainingFile), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenFileDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DeleteTrainingFileCommand { Id = Guid.NewGuid() };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiTrainingFiles.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiTrainingFile, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((AiTrainingFile)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
