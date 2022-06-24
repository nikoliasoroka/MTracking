using Microsoft.EntityFrameworkCore;
using MTracking.Core.Entities;

namespace MTracking.DAL
{
    public sealed class MTrackingDbContext : DbContext
    {
        public MTrackingDbContext(DbContextOptions<MTrackingDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(60);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<TimeLog> TimeLogs { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<Description> Descriptions { get; set; }

        public DbSet<ExportTimeLog> ExportTimeLogs { get; set; }

        public DbSet<Import> Imports { get; set; }

        public DbSet<Timer> Timers { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<UserFilePin> UserFilePins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}