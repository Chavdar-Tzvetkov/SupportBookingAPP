using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Areas.Engineers.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Engineers.Controllers
{
    [Area("Engineers")]
    [Authorize(Roles = "Engineer")]
    public class EngineersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EngineersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<int?> GetEngineerId()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.EngineerId;
        }

        public async Task<IActionResult> Index()
        {
            var engineerId = await GetEngineerId();
            if (engineerId == null)
                return RedirectToAction("Error403", "Error");

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.SupportCategory)
                .Where(b => b.EngineerId == engineerId)
                .OrderBy(b => b.SlotStart)
                .ToListAsync();

            var notifications = await _context.Notifications
                .Include(n => n.Booking)
                .Where(n => n.Booking.EngineerId == engineerId)
                .ToListAsync();

            var model = new EngineerDashboardViewModel
            {
                Bookings = bookings,
                Notifications = notifications
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var engineerId = await GetEngineerId();
            if (engineerId == null)
                return RedirectToAction("Error403", "Error");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.EngineerId == engineerId);
            if (booking == null)
                return NotFound();

            booking.IsCompleted = true;
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var engineerId = await GetEngineerId();
            if (engineerId == null)
                return RedirectToAction("Error403", "Error");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.EngineerId == engineerId);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var engineerId = await GetEngineerId();
            if (engineerId == null)
                return RedirectToAction("Error403", "Error");

            var booking = await _context.Bookings
                .Include(b => b.SupportCategory)
                .FirstOrDefaultAsync(b => b.Id == id && b.EngineerId == engineerId);

            if (booking == null || booking.IsCompleted)
                return NotFound();
            ViewBag.Categories = await _context.SupportCategories.ToListAsync();
            return View("Edit", booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking updated)
        {
            var engineerId = await GetEngineerId();
            if (engineerId == null || id != updated.Id)
                return RedirectToAction("Error403", "Error");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.EngineerId == engineerId);

            if (booking == null || booking.IsCompleted)
                return NotFound();

            if (!ModelState.IsValid)
                return View("Edit", updated);

            // Update only editable fields
            booking.SlotStart = updated.SlotStart;
            booking.SlotEnd = updated.SlotEnd;
            booking.IssueDescription = updated.IssueDescription;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");

        }

    }
}
