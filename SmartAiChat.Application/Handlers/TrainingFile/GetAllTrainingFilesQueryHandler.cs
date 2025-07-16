using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class GetAllTrainingFilesQueryHandler : IRequestHandler<GetAllTrainingFilesQuery, PaginatedResponse<AiTrainingFileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public GetAllTrainingFilesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<PaginatedResponse<AiTrainingFileDto>> Handle(GetAllTrainingFilesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var pagedResult = await _unitOfWork.AiTrainingFiles.GetPagedAsync(request.Pagination, cancellationToken);
            var trainingFileDtos = _mapper.Map<IEnumerable<AiTrainingFileDto>>(pagedResult.Items);
            return PaginatedResponse<AiTrainingFileDto>.Create(
                trainingFileDtos.ToList(),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize);
        }
    }
}
