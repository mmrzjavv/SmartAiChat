using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System.Text.Json;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class UploadTrainingFileCommandHandler : IRequestHandler<UploadTrainingFileCommand, AiTrainingFileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;
        private readonly IFileProcessingService _fileProcessingService;

        public UploadTrainingFileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext, IFileProcessingService fileProcessingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantContext = tenantContext;
            _fileProcessingService = fileProcessingService;
        }

        public async Task<AiTrainingFileDto> Handle(UploadTrainingFileCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var userId = Guid.NewGuid(); // Placeholder for user ID

            var filePath = await _fileProcessingService.SaveFileAsync(request.File.OpenReadStream(), request.File.FileName, tenantId, cancellationToken);
            var fileHash = await _fileProcessingService.CalculateFileHashAsync(request.File.OpenReadStream(), cancellationToken);

            var trainingFile = new AiTrainingFile
            {
                TenantId = tenantId,
                FileName = request.File.FileName,
                OriginalFileName = request.File.FileName,
                FilePath = filePath,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                FileHash = fileHash,
                UploadedByUserId = userId,
                Description = request.Description,
                Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null
            };

            await _unitOfWork.AiTrainingFiles.AddAsync(trainingFile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AiTrainingFileDto>(trainingFile);
        }
    }
}
