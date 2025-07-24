using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("Error403", "Error");

            var applicationDbContext = _context.Notifications.Include(n => n.Booking).ThenInclude(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var notification = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (notification == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isEngineer = notification.Booking?.Engineer?.Email == user.Email;
            var isOwner = notification.Booking?.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return RedirectToAction("Error403", "Error");

            return View(notification);
        }

        // GET: Notifications/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Id");
            return View();
        }

        // POST: Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,BookingId,NotifyAt,Sent")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Id", notification.BookingId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var notification = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isEngineer = notification.Booking?.Engineer?.Email == user.Email;
            var isOwner = notification.Booking?.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return RedirectToAction("Error403", "Error");

            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Id", notification.BookingId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingId,NotifyAt,Sent")] Notification notification)
        {
            if (id != notification.Id) return RedirectToAction("Error404", "Error");

            var existingNotification = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (existingNotification == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isEngineer = existingNotification.Booking?.Engineer?.Email == user.Email;
            var isOwner = existingNotification.Booking?.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return RedirectToAction("Error403", "Error");

            if (ModelState.IsValid)
            {
                try
                {
                    existingNotification.NotifyAt = notification.NotifyAt;
                    existingNotification.Sent = notification.Sent;
                    _context.Update(existingNotification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                        return RedirectToAction("Error404", "Error");
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Id", notification.BookingId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var notification = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (notification == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isEngineer = notification.Booking?.Engineer?.Email == user.Email;
            var isOwner = notification.Booking?.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return RedirectToAction("Error403", "Error");

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isEngineer = notification.Booking?.Engineer?.Email == user.Email;
            var isOwner = notification.Booking?.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return RedirectToAction("Error403", "Error");

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
