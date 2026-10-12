using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Name)
               .HasMaxLength(200);

        builder.Property(p => p.Description)
               .HasMaxLength(500);
    }
}
