using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.Handlers.TrainingFile;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.TrainingFile
{
    public class UploadTrainingFileCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly Mock<IFileProcessingService> _fileProcessingServiceMock;
        private readonly UploadTrainingFileCommandHandler _handler;

        public UploadTrainingFileCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _fileProcessingServiceMock = new Mock<IFileProcessingService>();
            _handler = new UploadTrainingFileCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _tenantContextMock.Object,
                _fileProcessingServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUploadTrainingFile()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var command = new UploadTrainingFileCommand { File = fileMock.Object };
            var trainingFile = new AiTrainingFile { TenantId = tenantId };
            var trainingFileDto = new SmartAiChat.Application.DTOs.AiTrainingFileDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _fileProcessingServiceMock.Setup(f => f.UploadFileAsync(fileMock.Object, It.IsAny<string>()))
                .ReturnsAsync("path/to/file");
            _mapperMock.Setup(m => m.Map<AiTrainingFile>(command)).Returns(trainingFile);
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.AiTrainingFileDto>(trainingFile)).Returns(trainingFileDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.AiTrainingFiles.Add(trainingFile), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(trainingFileDto, result);
        }
    }
}
