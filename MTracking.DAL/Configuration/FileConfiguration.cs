using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTracking.Core.Entities;

namespace MTracking.DAL.Configuration
{
    public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasIndex(x => x.PortfolioNumber)
                .IsUnique()
                .HasFilter(null);

            builder.HasMany(x => x.UserFilePins)
                .WithOne(x => x.File)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}