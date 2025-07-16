using MediatR;
using SmartAiChat.Application.Commands.TrainingFile;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.Application.Handlers.TrainingFile
{
    public class DeleteTrainingFileCommandHandler : IRequestHandler<DeleteTrainingFileCommand>
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
            var tenantId = _tenantContext.GetTenantId();
            var trainingFile = await _unitOfWork.AiTrainingFiles.GetFirstOrDefaultAsync(
                filter: f => f.Id == request.Id && f.TenantId == tenantId,
                cancellationToken: cancellationToken);

            if (trainingFile == null)
            {
                throw new Exception("Training File not found.");
            }

            // TODO: Implement file deletion from MinIO
            // await _fileProcessingService.DeleteFileAsync(trainingFile.FilePath);

            _unitOfWork.AiTrainingFiles.Remove(trainingFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
