using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Engineer> Engineers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<SupportCategory> SupportCategories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Booking → Engineer 
            builder.Entity<Booking>()
                .HasOne(b => b.Engineer)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EngineerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking → ApplicationUser
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser → Engineer 
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Engineer)
                .WithMany() // no reverse navigation
                .HasForeignKey(u => u.EngineerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }

}
