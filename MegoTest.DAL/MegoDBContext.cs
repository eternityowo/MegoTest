using MegoTest.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace MegoTest.DAL
{
    public class MegoDbContext : DbContext
    {
        public MegoDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Log> Log { get; set; }
        public DbSet<Metric> Metrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LogModelBuilderHelper.Build(modelBuilder.Entity<Log>());

            modelBuilder.Entity<Metric>().ToTable("Metrics");
            modelBuilder.Entity<Metric>().HasKey(m => m.Id).IsClustered(false);

#pragma warning disable CS0618 // Type or member is obsolete
            modelBuilder.Entity<Metric>().HasIndex("TaskName", "TimeInMs").HasName("IX_LogExtend").IsClustered();
#pragma warning restore CS0618 // Type or member is obsolete

            modelBuilder.Entity<Metric>().Property(u => u.TaskName).HasMaxLength(255);

        }
    }
}
