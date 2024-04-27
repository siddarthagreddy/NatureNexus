using Microsoft.EntityFrameworkCore;
using NatureNexus.Models;

namespace NatureNexus.Data
{
    public class NatureNexusContext : DbContext
    {
        public NatureNexusContext(DbContextOptions<NatureNexusContext> options) : base(options)
        {
        }
        public DbSet<Park> Parks { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<ParkActivity> ParkActivities { get; set; }
        public DbSet<ParkTopic> ParkTopics { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StatePark> StateParks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkActivity>()
            .Property(f => f.ID)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<ParkTopic>()
            .Property(f => f.ID)
            .ValueGeneratedOnAdd();
        }
    }
}
