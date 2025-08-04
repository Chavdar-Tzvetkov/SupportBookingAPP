using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupportBookingAPP.Models;
using SupportBookingAPP.Services;

namespace SupportBookingAPP.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(IBookingService bookingService, UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAllAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _bookingService.GetByIdAsync(id.Value);
            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _bookingService.UserCanAccessBooking(user, booking))
                return Forbid();

            return View(booking);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["EngineerId"] = await _bookingService.GetEngineerSelectListAsync();
            ViewData["SupportCategoryId"] = new SelectList(_bookingService.GetAllCategories(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EngineerId"] = await _bookingService.GetEngineerSelectListAsync();
                ViewData["SupportCategoryId"] = new SelectList(_bookingService.GetAllCategories(), "Id", "Name");
                return View(booking);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _bookingService.CreateAsync(booking, user);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _bookingService.GetByIdAsync(id.Value);
            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _bookingService.UserCanAccessBooking(user, booking))
                return Forbid();

            ViewData["EngineerId"] = await _bookingService.GetEngineerSelectListAsync();
            ViewData["SupportCategoryId"] = new SelectList(_bookingService.GetAllCategories(), "Id", "Name", booking.SupportCategoryId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "Email", booking.UserId);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.Id) return RedirectToAction("Error404", "Error");

            var original = await _bookingService.GetByIdAsync(id);
            if (original == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _bookingService.UserCanAccessBooking(user, original))
                return Forbid();

            original.SlotStart = booking.SlotStart;
            original.SlotEnd = booking.SlotEnd;
            original.IssueDescription = booking.IssueDescription;
            original.SupportCategoryId = booking.SupportCategoryId;

            if (User.IsInRole("Admin"))
            {
                original.EngineerId = booking.EngineerId;
                original.UserId = booking.UserId;
            }

            await _bookingService.UpdateAsync(original);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction("Error404", "Error");

            var booking = await _bookingService.GetByIdAsync(id.Value);
            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _bookingService.UserCanAccessBooking(user, booking))
                return Forbid();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return RedirectToAction("Error404", "Error");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _bookingService.UserCanAccessBooking(user, booking))
                return Forbid();

            await _bookingService.DeleteAsync(booking);
            return RedirectToAction(nameof(Index));
        }
    }
}
