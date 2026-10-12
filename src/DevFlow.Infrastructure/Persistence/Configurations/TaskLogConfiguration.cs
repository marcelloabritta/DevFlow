using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations;

public class TaskLogConfiguration : IEntityTypeConfiguration<TaskLog>
{
    public void Configure(EntityTypeBuilder<TaskLog> builder)
    {
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(tl => tl.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TaskItem>()
               .WithMany()
               .HasForeignKey(tl => tl.TaskItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(tl => tl.HoursLogged)
               .HasPrecision(4, 2);
    }
}
