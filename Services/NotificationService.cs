using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services; // This is where IEmailSender comes from
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Services
{
    public class NotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEmailSender _emailSender;

        public NotificationService(IServiceScopeFactory scopeFactory, IEmailSender emailSender)
        {
            _scopeFactory = scopeFactory;
            _emailSender = emailSender;
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

                await _emailSender.SendEmailAsync(user.Email!,
                    "Upcoming Support Slot",
                    $"Reminder: your slot with {eng.Name} at {n.Booking.SlotStart:G} is coming up.");

                n.Sent = true;
            }

            await ctx.SaveChangesAsync();
        }
    }
}
