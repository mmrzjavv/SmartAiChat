namespace SmartAiChat.Domain.Interfaces;

public interface IFileProcessingService
{
    Task<string> ExtractTextFromFileAsync(string filePath, string contentType, CancellationToken cancellationToken = default);
    Task<bool> IsValidFileTypeAsync(string contentType);
    Task<string> CalculateFileHashAsync(Stream fileStream, CancellationToken cancellationToken = default);
    Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task ProcessTrainingFileAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<List<string>> ChunkTextAsync(string text, int maxChunkSize = 1000, int overlapSize = 100);
} 