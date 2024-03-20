using Microsoft.EntityFrameworkCore;
using noted_database.Models;


namespace noted_database.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Character> Characters { get; set; }

         public DbSet<CampaignParticipant> CampaignParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships and other model configurations here
        }
    }
}
