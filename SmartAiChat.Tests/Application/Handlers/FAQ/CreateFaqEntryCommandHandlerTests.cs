using AutoMapper;
using Moq;
using SmartAiChat.Application.Commands.FAQ;
using SmartAiChat.Application.Handlers.FAQ;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Tests.Application.Handlers.FAQ
{
    public class CreateFaqEntryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly CreateFaqEntryCommandHandler _handler;

        public CreateFaqEntryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new CreateFaqEntryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateFaqEntry()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new CreateFaqEntryCommand { Question = "Test Question", Answer = "Test Answer" };
            var faqEntry = new FaqEntry { TenantId = tenantId, Question = "Test Question", Answer = "Test Answer" };
            var faqEntryDto = new SmartAiChat.Application.DTOs.FaqEntryDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _mapperMock.Setup(m => m.Map<FaqEntry>(command)).Returns(faqEntry);
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.FaqEntryDto>(faqEntry)).Returns(faqEntryDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.FaqEntries.Add(faqEntry), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(faqEntryDto, result);
        }
    }
}
