using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTracking.Core.Entities;

namespace MTracking.DAL.Configuration
{
    public class TimeLogConfiguration : IEntityTypeConfiguration<TimeLog>
    {
        public void Configure(EntityTypeBuilder<TimeLog> builder)
        {
            builder.HasIndex(x => x.CommitRecordId)
                .HasFilter(null);

            builder.HasOne(t => t.File)
                .WithMany(f => f.TimeLogs)
                .HasForeignKey(t => t.FileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.Topic)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(t => t.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.Description)
                .WithMany(d => d.TimeLogs)
                .HasForeignKey(t => t.DescriptionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
