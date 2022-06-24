using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTracking.Core.Entities;

namespace MTracking.DAL.Configuration
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.Property(entity => entity.ApplicationType)
                .IsRequired();

            builder.Property(entity => entity.FirebaseToken)
                .IsRequired();
        }
    }
}
