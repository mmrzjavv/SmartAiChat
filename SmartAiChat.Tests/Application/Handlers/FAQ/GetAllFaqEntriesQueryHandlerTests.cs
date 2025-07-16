using AutoMapper;
using Moq;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Handlers.FAQ;
using SmartAiChat.Application.Queries.FAQ;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Tests.Application.Handlers.FAQ
{
    public class GetAllFaqEntriesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GetAllFaqEntriesQueryHandler _handler;

        public GetAllFaqEntriesQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _tenantContextMock = new Mock<ITenantContext>();
            _handler = new GetAllFaqEntriesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _tenantContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedFaqEntries()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new GetAllFaqEntriesQuery { Pagination = new PaginationRequest { Page = 1, PageSize = 10 } };
            var faqEntries = new List<FaqEntry> { new FaqEntry { TenantId = tenantId } };
            var faqEntryDtos = new List<FaqEntryDto> { new FaqEntryDto() };
            var paginatedResult = new PaginatedResult<FaqEntryDto>(faqEntryDtos, 1, 1, 10);
            _tenantContextMock.Setup(c => c.GetTenantId()).Returns(tenantId);
            _unitOfWorkMock.Setup(u => u.FaqEntries.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FaqEntry, bool>>>(),
                null,
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((faqEntries, 1));
            _mapperMock.Setup(m => m.Map<IEnumerable<FaqEntryDto>>(faqEntries)).Returns(faqEntryDtos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedResult.Data, result.Data);
        }
    }
}
