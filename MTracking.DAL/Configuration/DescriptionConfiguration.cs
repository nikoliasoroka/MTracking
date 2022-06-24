using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTracking.Core.Entities;

namespace MTracking.DAL.Configuration
{
    public class DescriptionConfiguration : IEntityTypeConfiguration<Description>
    {
        public void Configure(EntityTypeBuilder<Description> builder)
        {
            builder.HasIndex(x => x.CommitRecordId)
                .HasFilter(null);
        }
    }
}
