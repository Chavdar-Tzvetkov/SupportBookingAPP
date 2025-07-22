using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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

            // Create roles if they don't exist
            string[] roles = { "Administrator", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User from secrets
            var adminEmail = adminSettings.Email;
            var adminPassword = adminSettings.Password;
            var adminRole = string.IsNullOrEmpty(adminSettings.Role) ? "Administrator" : adminSettings.Role;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, adminRole);
                }
                else
                {
                    throw new Exception("Failed to create Admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
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

            // Seed 10+ normal users if not present
            var existingUserCount = userManager.Users.Count(u => u.Email != adminEmail);
            if (existingUserCount < 10)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var email = $"user{i}@softuni.bg";
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(user, "Passw0rd!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, "User");
                        }
                    }
                }
            }

            // Seed 10+ bookings if not present
            if (!context.Bookings.Any())
            {
                var allUsers = userManager.Users.Where(u => u.Email != adminEmail).ToList();
                var allEngineers = context.Engineers.ToList();
                var rand = new Random();

                var bookings = new List<Booking>();
                for (int i = 0; i < 12; i++)
                {
                    var user = allUsers[rand.Next(allUsers.Count)];
                    var engineer = allEngineers[rand.Next(allEngineers.Count)];
                    var slotStart = DateTime.Today.AddDays(rand.Next(1, 10)).AddHours(rand.Next(8, 16));
                    var slotEnd = slotStart.AddHours(1);

                    bookings.Add(new Booking
                    {
                        UserId = user.Id,
                        EngineerId = engineer.Id,
                        SlotStart = slotStart,
                        SlotEnd = slotEnd,
                        IssueDescription = $"Auto-generated issue #{i + 1}"
                    });
                }

                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
            }
        }
    }
}
