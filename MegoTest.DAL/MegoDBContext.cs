using MegoTest.DAL.Entities;
using MegoTest.DAL.Models;
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


        public DbSet<MetricStat> MetricStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LogModelBuilderHelper.Build(modelBuilder.Entity<Log>());

            modelBuilder.Entity<Metric>().ToTable("Metrics");
            modelBuilder.Entity<Metric>().HasKey(m => m.Id).IsClustered(false);

            modelBuilder.Entity<Metric>().HasIndex("TaskName", "TimeInMs").HasDatabaseName("IX_LogExtend").IsClustered();

            modelBuilder.Entity<Metric>().Property(u => u.TaskName).HasMaxLength(255);

        }
    }
}
