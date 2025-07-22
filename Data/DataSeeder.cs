using Microsoft.AspNetCore.Identity;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Data
{
    public static class DataSeeder
    {
        public static async Task SeedInitialDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            //context.Database.Migrate();

            // Seed roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed engineers
            if (!context.Engineers.Any())
            {
                var engineers = new List<Engineer>
                {
                    new Engineer { Name = "Ivan Ivanov", Email = "ivan.ivanov@example.com" },
                    new Engineer { Name = "Maria Petrova", Email = "maria.petrova@example.com" },
                    new Engineer { Name = "Georgi Dimitrov", Email = "georgi.dimitrov@example.com" }
                };

                context.Engineers.AddRange(engineers);
                await context.SaveChangesAsync();
            }

            // Seed users
            if (!userManager.Users.Any())
            {
                var users = new[]
                {
                    new ApplicationUser { UserName = "alice@softuni.bg", Email = "alice@softuni.bg", EmailConfirmed = true },
                    new ApplicationUser { UserName = "bob@softuni.bg", Email = "bob@softuni.bg", EmailConfirmed = true }
                };

                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "Passw0rd!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // Seed bookings
            if (!context.Bookings.Any())
            {
                var alice = await userManager.FindByEmailAsync("alice@softuni.bg");
                var bob = await userManager.FindByEmailAsync("bob@softuni.bg");
                var engineer1 = context.Engineers.FirstOrDefault();
                var engineer2 = context.Engineers.Skip(1).FirstOrDefault();

                if (alice != null && bob != null && engineer1 != null && engineer2 != null)
                {
                    context.Bookings.AddRange(
                        new Booking
                        {
                            UserId = alice.Id,
                            EngineerId = engineer1.Id,
                            SlotStart = DateTime.Today.AddDays(1).AddHours(9),
                            SlotEnd = DateTime.Today.AddDays(1).AddHours(10),
                            IssueDescription = "Printer not working"
                        },
                        new Booking
                        {
                            UserId = bob.Id,
                            EngineerId = engineer2.Id,
                            SlotStart = DateTime.Today.AddDays(2).AddHours(13),
                            SlotEnd = DateTime.Today.AddDays(2).AddHours(14),
                            IssueDescription = "Email configuration issue"
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
