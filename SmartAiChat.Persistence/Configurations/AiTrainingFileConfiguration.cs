using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Persistence.Configurations;

public class AiTrainingFileConfiguration : IEntityTypeConfiguration<AiTrainingFile>
{
    public void Configure(EntityTypeBuilder<AiTrainingFile> builder)
    {
        builder.ToTable("AiTrainingFiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.TenantId).IsRequired();
        builder.Property(f => f.FileName).IsRequired();
        builder.Property(f => f.OriginalFileName).IsRequired();
        builder.Property(f => f.FilePath).IsRequired();
        builder.Property(f => f.ContentType).IsRequired();
        builder.Property(f => f.FileSize).IsRequired();
        builder.Property(f => f.FileHash).IsRequired();
        builder.Property(f => f.IsProcessed).IsRequired();
        builder.Property(f => f.IsActive).IsRequired();
        builder.Property(f => f.UploadedByUserId).IsRequired();
        builder.Property(f => f.HasEmbeddings).IsRequired();
        builder.Property(f => f.CreatedAt).IsRequired();
        builder.Property(f => f.IsDeleted).IsRequired();

        // Relationships
        builder.HasOne(f => f.Tenant)
            .WithMany(t => t.AiTrainingFiles)
            .HasForeignKey(f => f.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.UploadedBy)
            .WithMany()
            .HasForeignKey(f => f.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths
    }
} 