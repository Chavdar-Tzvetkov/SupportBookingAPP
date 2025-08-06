using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BookingService(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager,
                              IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .Include(b => b.SupportCategory)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .Include(b => b.SupportCategory)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> UserCanAccessBooking(ApplicationUser user, Booking booking)
        {
            return booking != null &&
                   (await _userManager.IsInRoleAsync(user, "Admin") ||
                    booking.Engineer?.Email == user.Email ||
                    booking.UserId == user.Id);
        }

        public async Task<List<SelectListItem>> GetEngineerSelectListAsync()
        {
            return await _context.Engineers
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Name })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSupportCategorySelectListAsync()
        {
            return await _context.SupportCategories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
        }

        public IEnumerable<SupportCategory> GetAllCategories()
        {
            return [.. _context.SupportCategories.AsNoTracking().OrderBy(c => c.Name)];
        }

        public async Task CreateAsync(Booking booking, ApplicationUser user)
        {
            booking.UserId = user.Id;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _context.Notifications.Add(new Notification
            {
                BookingId = booking.Id,
                Booking = booking,
                NotifyAt = booking.SlotStart.AddMinutes(-15), // or use any logic you prefer
                Sent = false
            });
            await _context.SaveChangesAsync(); 

            var engineer = await _context.Engineers.FindAsync(booking.EngineerId);
            if (engineer != null)
            {
                await _emailSender.SendEmailAsync(user.Email, "Booking Confirmed",
                    $"Hi {user.UserName},<br/>Your support slot with {engineer.Name} is confirmed for {booking.SlotStart:G}.");

                await _emailSender.SendEmailAsync(engineer.Email, "New Booking Assigned",
                    $"Hi {engineer.Name},<br/>You have a new booking from {user.Email} scheduled at {booking.SlotStart:G}.");
            }
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            _context.Notifications.Add(new Notification
            {
                BookingId = booking.Id,
                Booking = booking,
                NotifyAt = booking.SlotStart.AddMinutes(-15),
                Sent = false
            });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsCompletedAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return;

            booking.IsCompleted = true;
            _context.Bookings.Update(booking);
            _context.Notifications.Add(new Notification
            {
                BookingId = booking.Id,
                Booking = booking,
                NotifyAt = DateTime.UtcNow,
                Sent = false
            });
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Bookings.Any(b => b.Id == id);
        }
    }
}
