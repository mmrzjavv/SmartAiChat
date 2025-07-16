using MediatR;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class DeleteTrainingFileCommandHandler : IRequestHandler<DeleteTrainingFileCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantContext _tenantContext;
        private readonly IFileProcessingService _fileProcessingService;

        public DeleteTrainingFileCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext, IFileProcessingService fileProcessingService)
        {
            _unitOfWork = unitOfWork;
            _tenantContext = tenantContext;
            _fileProcessingService = fileProcessingService;
        }

        public async Task<Unit> Handle(DeleteTrainingFileCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantContext.TenantId;
            var trainingFile = await _unitOfWork.AiTrainingFiles.FirstOrDefaultAsync(
                f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken);

            if (trainingFile == null)
            {
                throw new Exception("Training File not found.");
            }

            // TODO: Implement file deletion from MinIO
            // await _fileProcessingService.DeleteFileAsync(trainingFile.FilePath);

            await _unitOfWork.AiTrainingFiles.DeleteAsync(trainingFile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
