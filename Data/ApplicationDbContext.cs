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

            // Seed Engineers with correct DateTime values
            builder.Entity<Engineer>().HasData(
                new Engineer
                {
                    Id = 1,
                    Name = "Alice Ivanova",
                    Specialty = "Networking",
                    WorkdayStart = DateTime.Today.AddHours(9),
                    WorkdayEnd = DateTime.Today.AddHours(17)
                },
                new Engineer
                {
                    Id = 2,
                    Name = "Boris Petrov",
                    Specialty = "Hardware",
                    WorkdayStart = DateTime.Today.AddHours(10),
                    WorkdayEnd = DateTime.Today.AddHours(18)
                }
            );
        }
    }
}
