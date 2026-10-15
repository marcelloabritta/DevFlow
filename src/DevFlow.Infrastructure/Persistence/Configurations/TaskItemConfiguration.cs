using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(t => t.Title)
               .HasMaxLength(200);

        builder.Property(t => t.Description)
               .HasMaxLength(500);

        builder.Property(t => t.LoggedHours)
               .HasPrecision(6, 2);

        builder.Property(t => t.EstimatedHours)
               .HasPrecision(5, 2);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(t => t.AssignedToUserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Project>()
               .WithMany()
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
