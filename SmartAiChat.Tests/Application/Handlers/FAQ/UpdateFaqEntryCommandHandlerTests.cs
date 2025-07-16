using AutoMapper;
using Moq;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Handlers.FAQ;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.FAQ
{
    public class UpdateFaqEntryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly UpdateFaqEntryCommandHandler _handler;

        public UpdateFaqEntryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new UpdateFaqEntryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateFaqEntry_WhenEntryExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new UpdateFaqEntryCommand { Id = Guid.NewGuid(), Question = "Updated Question", Answer = "Updated Answer" };
            var faqEntry = new FaqEntry { Id = command.Id, TenantId = tenantId };
            var faqEntryDto = new SmartAiChat.Application.DTOs.FaqEntryDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(faqEntry);
            _mapperMock.Setup(m => m.Map(command, faqEntry));
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.FaqEntryDto>(faqEntry)).Returns(faqEntryDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.FaqEntries.Update(faqEntry), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(faqEntryDto, result);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenEntryDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new UpdateFaqEntryCommand { Id = Guid.NewGuid() };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((FaqEntry)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
