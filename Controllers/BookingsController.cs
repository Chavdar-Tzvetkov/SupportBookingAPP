using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using SupportBookingAPP.Services;

namespace SupportBookingAPP.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

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
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge(); // Not logged in
                }

                booking.UserId = user.Id;
                _context.Add(booking);
                await _context.SaveChangesAsync();

                // ✅ BEGIN EMAIL LOGIC
                var engineer = await _context.Engineers.FindAsync(booking.EngineerId);

                var emailService = HttpContext.RequestServices.GetRequiredService<EmailService>();

                // To User
                await emailService.SendEmailAsync(user.Email,
                    "Booking Confirmed",
                    $"Hi {user.UserName},<br/>Your support slot with {engineer.Name} is confirmed for {booking.SlotStart:G}.");

                // To Engineer
                await emailService.SendEmailAsync(engineer.Email,
                    "New Booking Assigned",
                    $"Hi {engineer.Name},<br/>You have a new booking from {user.Email} scheduled at {booking.SlotStart:G}.");
                // ✅ END EMAIL LOGIC

                return RedirectToAction(nameof(Index));
            }

            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name", booking.EngineerId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name", booking.EngineerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,EngineerId,SlotStart,SlotEnd,IssueDescription")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EngineerId"] = new SelectList(_context.Engineers, "Id", "Name", booking.EngineerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Engineer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
