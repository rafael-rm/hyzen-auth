using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasColumnType("INT")
            .ValueGeneratedOnAdd();
            
        builder.Property(u => u.Guid)
            .HasColumnName("guid")
            .HasColumnType("UUID")
            .IsRequired();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}