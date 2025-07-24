using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SupportBookingAPP.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BookingsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = User.IsInRole("Admin");
            var isEngineer = booking.Engineer?.Email == user.Email;
            var isOwner = booking.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return Forbid();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EngineerId,SlotStart,SlotEnd,IssueDescription")] Booking booking)
        {
            if (!ModelState.IsValid)
                return View(booking);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            booking.UserId = user.Id;
            _context.Add(booking);
            await _context.SaveChangesAsync();

            var engineer = await _context.Engineers.FindAsync(booking.EngineerId);

            if (engineer != null)
            {
                // Email confirmations
                await _emailSender.SendEmailAsync(user.Email,
                    "Booking Confirmed",
                    $"Hi {user.UserName},<br/>Your support slot with {engineer.Name} is confirmed for {booking.SlotStart:G}.");

                await _emailSender.SendEmailAsync(engineer.Email,
                    "New Booking Assigned",
                    $"Hi {engineer.Name},<br/>You have a new booking from {user.Email} scheduled at {booking.SlotStart:G}.");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = User.IsInRole("Admin");
            var isEngineer = booking.Engineer?.Email == user.Email;
            var isOwner = booking.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return Forbid();

            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name", booking.EngineerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,EngineerId,SlotStart,SlotEnd,IssueDescription")] Booking booking)
        {
            if (id != booking.Id) return RedirectToAction("Error404", "Error");

            var existingBooking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = User.IsInRole("Admin");
            var isEngineer = existingBooking.Engineer?.Email == user.Email;
            var isOwner = existingBooking.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    existingBooking.SlotStart = booking.SlotStart;
                    existingBooking.SlotEnd = booking.SlotEnd;
                    existingBooking.IssueDescription = booking.IssueDescription;

                    if (isAdmin)
                    {
                        existingBooking.EngineerId = booking.EngineerId;
                        existingBooking.UserId = booking.UserId;
                    }

                    _context.Update(existingBooking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id)) return RedirectToAction("Error404", "Error");
                    throw;
                }
            }

            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name", booking.EngineerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = User.IsInRole("Admin");
            var isEngineer = booking.Engineer?.Email == user.Email;
            var isOwner = booking.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return Forbid();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = User.IsInRole("Admin");
            var isEngineer = booking.Engineer?.Email == user.Email;
            var isOwner = booking.UserId == user.Id;

            if (!isAdmin && !isEngineer && !isOwner)
                return Forbid();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
