using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.AIConfiguration;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.AIConfiguration
{
    public class GetAiConfigurationQueryHandler : IRequestHandler<GetAIConfigurationQuery, AiConfigurationDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public GetAiConfigurationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<AiConfigurationDto?> Handle(GetAIConfigurationQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var aiConfiguration = await _unitOfWork.AiConfigurations.FirstOrDefaultAsync(
                c => c.TenantId == tenantId,
                cancellationToken);

            if (aiConfiguration == null)
            {
                // Optionally create a default configuration if one doesn't exist
                // For now, we'll return null or a default DTO
                return null;
            }

            return _mapper.Map<AiConfigurationDto>(aiConfiguration);
        }
    }
}
