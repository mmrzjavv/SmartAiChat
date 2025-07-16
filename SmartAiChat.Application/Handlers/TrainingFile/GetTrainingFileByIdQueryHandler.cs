using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.TrainingFile;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class GetTrainingFileByIdQueryHandler : IRequestHandler<GetTrainingFileByIdQuery, AiTrainingFileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public GetTrainingFileByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<AiTrainingFileDto> Handle(GetTrainingFileByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.GetTenantId();
            var trainingFile = await _unitOfWork.AiTrainingFiles.GetFirstOrDefaultAsync(
                filter: f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken: cancellationToken);

            if (trainingFile == null)
            {
                return null;
            }

            return _mapper.Map<AiTrainingFileDto>(trainingFile);
        }
    }
}
