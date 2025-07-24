using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public List<UserWithRoles>? Users { get; set; }
        public List<Engineer>? Engineers { get; set; }
        public List<Booking>? Bookings { get; set; }
    }

    public class UserWithRoles
    {
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }
}
