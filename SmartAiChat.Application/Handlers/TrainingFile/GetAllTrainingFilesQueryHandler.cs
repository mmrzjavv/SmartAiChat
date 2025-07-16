using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class GetAllTrainingFilesQueryHandler : IRequestHandler<GetAllTrainingFilesQuery, PaginatedResult<AiTrainingFileDto>>
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

        public async Task<PaginatedResult<AiTrainingFileDto>> Handle(GetAllTrainingFilesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.GetTenantId();
            var (trainingFiles, totalCount) = await _unitOfWork.AiTrainingFiles.GetAsync(
                filter: f => f.TenantId == tenantId,
                page: request.Pagination.Page,
                pageSize: request.Pagination.PageSize,
                cancellationToken: cancellationToken);

            var trainingFileDtos = _mapper.Map<IEnumerable<AiTrainingFileDto>>(trainingFiles);

            return new PaginatedResult<AiTrainingFileDto>(
                trainingFileDtos,
                totalCount,
                request.Pagination.Page,
                request.Pagination.PageSize);
        }
    }
}
