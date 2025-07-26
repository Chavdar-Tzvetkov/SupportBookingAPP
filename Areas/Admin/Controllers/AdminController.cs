using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Areas.Admin.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();
            var userWithRoles = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles;
                userWithRoles.Add(new UserWithRoles
                {
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            var engineers = await _context.Engineers.ToListAsync();
            var categories = await _context.SupportCategories.ToListAsync();
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Engineer)
                .OrderByDescending(b => b.SlotStart)
                .Take(10)
                .ToListAsync();

            ViewBag.UserRoles = userRoles;

            var dashboardModel = new AdminDashboardViewModel
            {
                Users = userWithRoles,
                Engineers = engineers,
                Categories = categories,
                Bookings = recentBookings
            };

            return View(dashboardModel);
        }
    }
}
