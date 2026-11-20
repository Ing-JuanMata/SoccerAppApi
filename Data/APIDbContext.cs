using Microsoft.EntityFrameworkCore;
using API.Models;
namespace API.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Team> Team { get; set; }
        public DbSet<League> League { get; set; }
        public DbSet<TeamLeague> TeamLeague { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TeamLeague>()
                .HasKey(x => new
                {
                    x.IdTeam,
                    x.IdLeague
                });
            modelBuilder.Entity<TeamLeague>()
                .HasOne(x => x.League)
                .WithMany(x => x.TeamLeagues)
                .HasForeignKey(x => x.IdLeague);
            modelBuilder.Entity<TeamLeague>()
                .HasOne(x => x.Team)
                .WithMany(x => x.TeamLeagues)
                .HasForeignKey(x => x.IdTeam);
        }
    }
}