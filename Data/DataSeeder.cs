using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
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
            var adminSettings = services.GetRequiredService<IOptions<AdminUserSettings>>().Value;

            // Ensure DB is created
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin
            var adminEmail = adminSettings.Email;
            var adminPassword = adminSettings.Password;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                    throw new Exception("Failed to create Admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Support Categories
            if (!context.SupportCategories.Any())
            {
                var categories = new[]
                {
                    new SupportCategory { Name = "Hardware" },
                    new SupportCategory { Name = "Software" },
                    new SupportCategory { Name = "Email" },
                    new SupportCategory { Name = "Networking" },
                };

                context.SupportCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Engineers
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

            // Seed Regular Users
            for (int i = 1; i <= 10; i++)
            {
                var email = $"user{i}@softuni.bg";
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "Passw0rd!");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, "User");
                }
            }

            // Ensure Users and Engineers are reloaded
            var allUsers = await userManager.Users.Where(u => u.Email != adminEmail).ToListAsync();
            var allEngineers = await context.Engineers.ToListAsync();

            // Seed Bookings
            if (!context.Bookings.Any() && allUsers.Count > 0 && allEngineers.Count > 0)
            {
                var rand = new Random();
                var bookings = new List<Booking>();

                var defaultCategory = context.SupportCategories.First(); 

                for (int i = 0; i < 12; i++)
                {
                    var user = allUsers[rand.Next(allUsers.Count)];
                    var engineer = allEngineers[rand.Next(allEngineers.Count)];
                    var startHour = rand.Next(9, 17);
                    var slotStart = DateTime.Today.AddDays(rand.Next(1, 10)).AddHours(startHour);
                    var slotEnd = slotStart.AddHours(1);

                    bookings.Add(new Booking
                    {
                        UserId = user.Id,
                        EngineerId = engineer.Id,
                        SlotStart = slotStart,
                        SlotEnd = slotEnd,
                        IssueDescription = $"Auto-generated issue #{i + 1}",
                        SupportCategoryId = defaultCategory.Id 
                    });
                }


                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
            }
        }
    }
}
