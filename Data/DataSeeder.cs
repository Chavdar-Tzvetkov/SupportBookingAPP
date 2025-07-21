using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Data
{
    public static class DataSeeder
    {
        public static void SeedInitialData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

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
                context.SaveChanges();
            }
        }
    }
}
