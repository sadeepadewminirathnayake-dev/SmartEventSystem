using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;           // ← must have this

namespace SmartEventSystem.Models
{
    public class ApplicationDbContext : DbContext     // ← Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Your DbSets here
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Venue> Venues { get; set; }

        



        // Optional: if you want to configure relationships / keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
        }
    }
}
