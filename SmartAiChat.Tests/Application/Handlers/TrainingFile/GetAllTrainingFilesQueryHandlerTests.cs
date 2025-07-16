using AutoMapper;
using Moq;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Handlers.TrainingFile;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Tests.Application.Handlers.TrainingFile
{
    public class GetAllTrainingFilesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GetAllTrainingFilesQueryHandler _handler;

        public GetAllTrainingFilesQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new GetAllTrainingFilesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedTrainingFiles()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new GetAllTrainingFilesQuery { Pagination = new PaginationRequest { Page = 1, PageSize = 10 } };
            var trainingFiles = new List<AiTrainingFile> { new AiTrainingFile { TenantId = tenantId } };
            var trainingFileDtos = new List<AiTrainingFileDto> { new AiTrainingFileDto() };
            var paginatedResult = new PaginatedResult<AiTrainingFileDto>(trainingFileDtos, 1, 1, 10);
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.AiTrainingFiles.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AiTrainingFile, bool>>>(),
                null,
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((trainingFiles, 1));
            _mapperMock.Setup(m => m.Map<IEnumerable<AiTrainingFileDto>>(trainingFiles)).Returns(trainingFileDtos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedResult.Data, result.Data);
        }
    }
}
