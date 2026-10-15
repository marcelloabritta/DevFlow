using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name)
               .HasMaxLength(200);

        builder.Property(u => u.Email)
               .HasMaxLength(254);

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
