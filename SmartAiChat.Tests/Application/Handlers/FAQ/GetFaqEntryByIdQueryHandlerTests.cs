using AutoMapper;
using Moq;
using SmartAiChat.Application.Handlers.FAQ;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartAiChat.Tests.Application.Handlers.FAQ
{
    public class GetFaqEntryByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GetFaqEntryByIdQueryHandler _handler;

        public GetFaqEntryByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new GetFaqEntryByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFaqEntry_WhenEntryExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var query = new GetFaqEntryByIdQuery { Id = Guid.NewGuid() };
            var faqEntry = new FaqEntry { Id = query.Id, TenantId = tenantId };
            var faqEntryDto = new SmartAiChat.Application.DTOs.FaqEntryDto();
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(faqEntry);
            _mapperMock.Setup(m => m.Map<SmartAiChat.Application.DTOs.FaqEntryDto>(faqEntry)).Returns(faqEntryDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(faqEntryDto, result);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenEntryDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var query = new GetFaqEntryByIdQuery { Id = Guid.NewGuid() };
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((FaqEntry)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
