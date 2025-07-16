using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.AIConfiguration;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.AIConfiguration
{
    public class UpdateAiConfigurationCommandHandler : IRequestHandler<UpdateAiConfigurationCommand, AiConfigurationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;

        public UpdateAiConfigurationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
        }

        public async Task<AiConfigurationDto> Handle(UpdateAiConfigurationCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var aiConfiguration = await _unitOfWork.AiConfigurations.FirstOrDefaultAsync(
                c => c.TenantId == tenantId,
                cancellationToken);

            if (aiConfiguration == null)
            {
                // Handle case where configuration does not exist
                // For now, we'll throw an exception or return null
                // Depending on desired behavior
                throw new Exception("AI Configuration not found for this tenant.");
            }

            _mapper.Map(request, aiConfiguration);
            await _unitOfWork.AiConfigurations.UpdateAsync(aiConfiguration, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AiConfigurationDto>(aiConfiguration);
        }
    }
}
