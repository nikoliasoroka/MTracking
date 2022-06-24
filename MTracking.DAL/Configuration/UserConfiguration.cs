using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTracking.Core.Entities;

namespace MTracking.DAL.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.CommitId)
                .IsUnique();

            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.HasMany(t => t.Topics)
                .WithOne(f => f.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TimeLogs)
                .WithOne(f => f.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(d => d.Descriptions)
                .WithOne(f => f.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Devices)
                .WithOne(f => f.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
