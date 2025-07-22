using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Services
{
    public class NotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailService _emailService;

        public NotificationService(IServiceScopeFactory scopeFactory, EmailService emailService)
        {
            _scopeFactory = scopeFactory;
            _emailService = emailService;
        }

        public async Task RunPendingAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.Now;
            var pending = await ctx.Notifications
                .Include(n => n.Booking).ThenInclude(b => b.User)
                .Include(n => n.Booking).ThenInclude(b => b.Engineer)
                .Where(n => !n.Sent && n.NotifyAt <= now)
                .ToListAsync();

            foreach (var n in pending)
            {
                var user = n.Booking.User!;
                var eng = n.Booking.Engineer!;

                await _emailService.SendEmailAsync(user.Email!,
                    "Upcoming Support Slot",
                    $"Reminder: your slot with {eng.Name} at {n.Booking.SlotStart:G} is coming up.");

                n.Sent = true;
            }

            await ctx.SaveChangesAsync();
        }
    }
}
