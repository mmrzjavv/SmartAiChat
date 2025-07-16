using Moq;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Handlers.FAQ;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.FAQ
{
    public class DeleteFaqEntryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly DeleteFaqEntryCommandHandler _handler;

        public DeleteFaqEntryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new DeleteFaqEntryCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteFaqEntry_WhenEntryExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DeleteFaqEntryCommand { Id = Guid.NewGuid() };
            var faqEntry = new FaqEntry { Id = command.Id, TenantId = tenantId };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(faqEntry);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.FaqEntries.Remove(faqEntry), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenEntryDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DeleteFaqEntryCommand { Id = Guid.NewGuid() };
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
