using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Data.Configurations
{
    public class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
    {
        public void Configure(EntityTypeBuilder<GroupRole> builder)
        {
            builder.ToTable("group_roles");

            builder.HasKey(gr => new { gr.GroupId, gr.RoleId });

            builder.Property(gr => gr.GroupId)
                .HasColumnName("group_id")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(gr => gr.RoleId)
                .HasColumnName("role_id")
                .HasColumnType("INT")
                .IsRequired();

            builder.HasOne(gr => gr.Group)
                .WithMany(g => g.GroupRoles)
                .HasForeignKey(gr => gr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gr => gr.Role)
                .WithMany(r => r.GroupRoles)
                .HasForeignKey(gr => gr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}