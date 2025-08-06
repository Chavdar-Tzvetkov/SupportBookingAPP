using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Areas.Engineers.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Engineers.Controllers
{
    [Area("Engineers")]
    [Authorize(Roles = "Engineer")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.EngineerId == null)
                return RedirectToAction("Error403", "Error", new { area = "" });

            var engineerId = user.EngineerId.Value;

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.SupportCategory)
                .Where(b => b.EngineerId == engineerId)
                .OrderBy(b => b.SlotStart)
                .ToListAsync();

            var notifications = await _context.Notifications
                .Include(n => n.Booking)
                .ThenInclude(b => b.User)
                .Where(n => n.Booking.EngineerId == engineerId)
                .OrderByDescending(n => n.NotifyAt)
                .ToListAsync();

            var model = new EngineerDashboardViewModel
            {
                Bookings = bookings,
                Notifications = notifications
            };

            return View(model);
        }
    }
}
